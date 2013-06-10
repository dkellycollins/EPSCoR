using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using WebMatrix.WebData;
using System.Web.Script.Serialization;
using EPSCoR.Repositories.Basic;
using EPSCoR.ViewModels;

namespace EPSCoR.Controllers
{
    //[Authorize]
    public class FilesController : BootstrapBaseController
    {
        private IModelRepository<TableIndex> _tableIndexRepo;
        private IFileAccessor _uploadFileAccessor;
        private IFileAccessor _conversionFileAccessor;
        private IFileAccessor _tempFileAccessor;

        public FilesController()
        {
            _tableIndexRepo = new BasicModelRepo<TableIndex>();
            _uploadFileAccessor = BasicFileAccessor.GetUploadAccessor(WebSecurity.CurrentUserName);
            _conversionFileAccessor = BasicFileAccessor.GetConversionsAccessor(WebSecurity.CurrentUserName);
            _tempFileAccessor = BasicFileAccessor.GetTempAccessor(WebSecurity.CurrentUserName);
        }

        public FilesController(
            IModelRepository<TableIndex> tableIndexRepo,
            IFileAccessor uploadFileAccessor,
            IFileAccessor conversionFileAccessor,
            IFileAccessor tempFileAccessor)
        {
            _tableIndexRepo = tableIndexRepo;
            _uploadFileAccessor = uploadFileAccessor;
            _conversionFileAccessor = conversionFileAccessor;
            _tempFileAccessor = tempFileAccessor;
        }

        //
        // GET: /Upload/

        public ActionResult Upload()
        {
            List<string> fileNames = new List<string>();
            foreach (string fullFilePath in _uploadFileAccessor.GetFiles())
                fileNames.Add(Path.GetFileName(fullFilePath));
            return View(fileNames);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            string userName = WebSecurity.CurrentUserName;
            string tableName = Path.GetFileNameWithoutExtension(file.FileName);
            TableIndex existingTable = _tableIndexRepo.GetAll().Where(t => t.Name == tableName && t.UploadedByUser == userName).FirstOrDefault();
            if (existingTable != null)
            {
                DisplayAttention("Table already exist. Remove existing table before uploading the new one.");
                return RedirectToAction("Upload");
            }

            bool saveSuccessfull = _uploadFileAccessor.SaveFiles(FileStreamWrapper.FromHttpPostedFile(file));

            if (saveSuccessfull)
            {
                DisplaySuccess("Upload Successful!");
            }
            else
            {
                DisplayError("Upload failed.");
            }

            return RedirectToAction("Index");
        }

        //
        // POST: /Table/Upload/
        [HttpPost]
        public ActionResult UploadFiles(FileUpload file)
        {
            string fileName = Path.GetFileName(file.FileName);
            string tableName = Path.GetFileNameWithoutExtension(file.FileName);
            string userName = WebSecurity.CurrentUserName;
            TableIndex existingTable = _tableIndexRepo.GetAll().Where(t => t.Name == tableName && t.UploadedByUser == userName).FirstOrDefault();
            if (existingTable != null)
            {
                return new FileUploadResult(fileName, "Table already exists. Delete exisiting table before uploading a new one.");
            }

            //Save the chunk.
            FileStreamWrapper wrapper = new FileStreamWrapper()
            {
                FileName = fileName,
                InputStream = file.InputStream,
                SeekPos = file.StartPosition,
                FileSize = file.TotalFileLength
            };

            bool saveSuccessful = _tempFileAccessor.SaveFiles(wrapper);

            return new FileUploadResult(fileName);
        }

        [HttpPost]
        public ActionResult CompleteUpload(string id)
        {
            bool result = false;
            FileStream fileStream = _tempFileAccessor.OpenFile(id);
            if (fileStream != null)
            {
                FileStreamWrapper wrapper = new FileStreamWrapper()
                {
                    FileName = id,
                    InputStream = fileStream
                };
                result = _uploadFileAccessor.SaveFiles(wrapper);
                _tempFileAccessor.CloseFile(fileStream);
                _tempFileAccessor.DeleteFiles(id);
            }

            if (result)
                return new FileUploadResult(id);
            return new FileUploadResult(id, "Could not complete file upload.");
        }

        public ActionResult Download()
        {
            return View(createFileDownloadVM(_tableIndexRepo.GetAll()));
        }

        public ActionResult DownloadCsv(string id)
        {
            string fileName = id + ".csv";
            if(!_conversionFileAccessor.FileExist(fileName))
            {
                return new HttpNotFoundResult();
            }

            var cd = new ContentDisposition
            {
                FileName = fileName,
                Inline = false
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(_conversionFileAccessor.OpenFile(fileName), "text/csv");
        }

        public void Dispose()
        {
            _tableIndexRepo.Dispose();
        }

        #region Helpers

        private List<FileDownloadVM> createFileDownloadVM(IEnumerable<TableIndex> tableIndexes)
        {
            List<FileDownloadVM> returnList = new List<FileDownloadVM>();
            foreach (TableIndex tableIndex in tableIndexes)
            {
                if (!_conversionFileAccessor.FileExist(tableIndex.Name + ".csv"))
                    continue;

                returnList.Add(new FileDownloadVM()
                {
                    Table = tableIndex.Name,
                    Type = tableIndex.Type,
                    DateCreated = tableIndex.DateUpdated.ToString(),
                    Issuer = tableIndex.UploadedByUser
                });
            }

            return returnList;
        }

        #endregion
    }

    [ModelBinder(typeof(ModelBinder))]
    public class FileUpload
    {
        public string FileName { get; set; }
        public Stream InputStream { get; set; }
        public int StartPosition { get; set; }
        public int TotalFileLength { get; set; }

        public class ModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var request = controllerContext.RequestContext.HttpContext.Request;

                string fileName = request.Files[0].FileName;
                Stream inputStream = request.Files[0].InputStream;
                int startPos;
                int totalFileLength;
                if (request.Headers["Content-Range"] != null)
                {
                    string[] fileInfo = request.Headers["Content-Range"].Split('/', '-');
                    startPos = Int32.Parse(fileInfo[0].Remove(0, 5));
                    totalFileLength = Int32.Parse(fileInfo[2]);
                }
                else
                {
                    startPos = 0;
                    totalFileLength = request.Files[0].ContentLength;
                }

                return new FileUpload()
                {
                    FileName = fileName,
                    InputStream = inputStream,
                    StartPosition = startPos,
                    TotalFileLength = totalFileLength
                };
            }
        }
    }

    public class FileUploadResult : ActionResult
    {
        /// <summary>
        /// This wraps up data to be serailized into the Json object returned.
        /// </summary>
        public class FileStatus
        {
            public string Name { get; set; }
            public string Error { get; set; }
        }

        private JavaScriptSerializer _serializer;
        private FileStatus _status;

        public FileUploadResult(FileStatus status)
        {
            _serializer = new JavaScriptSerializer();
            _status = status;
        }

        public FileUploadResult(string FileName, string error = null)
        {
            _serializer = new JavaScriptSerializer();
            _status = new FileStatus()
            {
                Name = FileName,
                Error = error
            };
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;

            response.AddHeader("Vary", "Accept");
            try
            {
                if (request["HTTP_ACCEPT"].Contains("application/json"))
                    response.ContentType = "application/json";
                else
                    response.ContentType = "text/plain";
            }
            catch
            {
                response.ContentType = "text/plain";
            }

            var jsonObj = _serializer.Serialize(_status);
            response.Write(jsonObj);
        }
    }
}
