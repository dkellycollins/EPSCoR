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

namespace EPSCoR.Controllers
{
    //[Authorize]
    public class FilesController : BootstrapBaseController
    {
        private IRepository<TableIndex> _tableIndexRepo;
        private IFileAccessor _uploadFileAccessor;
        private IFileAccessor _conversionFileAccessor;
        private IFileAccessor _tempFileAccessor;

        public FilesController()
        {
            _tableIndexRepo = new BasicRepo<TableIndex>();
            _uploadFileAccessor = BasicFileAccessor.GetUploadAccessor(WebSecurity.CurrentUserName);
            _conversionFileAccessor = BasicFileAccessor.GetConversionsAccessor(WebSecurity.CurrentUserName);
            _tempFileAccessor = BasicFileAccessor.GetTempAccessor(WebSecurity.CurrentUserName);
        }

        public FilesController(
            IRepository<TableIndex> tableIndexRepo,
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

        public ActionResult Index()
        {
            List<string> fileNames = new List<string>();
            foreach (string fullFilePath in _uploadFileAccessor.GetFiles())
                fileNames.Add(Path.GetFileName(fullFilePath));
            return View(fileNames);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            string tableName = Path.GetFileNameWithoutExtension(file.FileName);
            TableIndex existingTable = _tableIndexRepo.GetAll().Where(t => t.Name == tableName).FirstOrDefault();
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

            //Save the chunk.
            FileStreamWrapper wrapper = new FileStreamWrapper()
            {
                FileName = fileName,
                InputStream = file.InputStream
            };

            bool saveSuccessful;
            if (file.IsChunk)
            {
                saveSuccessful = _tempFileAccessor.SavePartialFiles(wrapper);

                //If this was the last chunk, save the complete file.
                if (saveSuccessful && file.IsLastChunk)
                {
                    //Check to see if this table already exis
                    TableIndex existingTable = _tableIndexRepo.GetAll().Where((t) => t.Name == fileName).FirstOrDefault();
                    if (existingTable != null)
                    {
                        return new FileUploadResult(fileName, "Table already exist. Remove existing table before uploading the new one.");
                    }

                    FileStream fileStream = _tempFileAccessor.OpenFile(fileName);
                    wrapper.InputStream = fileStream;
                    saveSuccessful = _uploadFileAccessor.SaveFiles(wrapper);
                    _tempFileAccessor.CloseFile(fileStream);
                    _tempFileAccessor.DeleteFiles(fileName);
                }
            }
            else
            {
                saveSuccessful = _uploadFileAccessor.SaveFiles(wrapper);
            }

            return new FileUploadResult(fileName);
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

        private bool mergeTempFiles(string fullFileName, int totalParts)
        {
            string fileName = Path.GetFileNameWithoutExtension(fullFileName);
            MemoryStream completeFileStream = new MemoryStream();
            byte[] b = new byte[1024];

            //Write each temp file to the stream.
            for (int i = 0; i < totalParts; i++)
            {
                FileStream tempFileStream = _tempFileAccessor.OpenFile(fileName + i);
                tempFileStream.CopyTo(completeFileStream);
                tempFileStream.Close();
            }
            //Set the stream back at the begining.
            completeFileStream.Seek(0, SeekOrigin.Begin);

            //Save complete file.
            FileStreamWrapper wrapper = new FileStreamWrapper()
            {
                FileName = fullFileName,
                InputStream = completeFileStream
            };
            return _uploadFileAccessor.SaveFiles(wrapper);
        }

        private void deleteTempFiles(string fileName, int totalParts)
        {
            string[] tempFileNames = new string[totalParts];
            for (int i = 0; i < totalParts; i++)
            {
                tempFileNames[i] = fileName + i;
            }
            _tempFileAccessor.DeleteFiles(tempFileNames);
        }
    }

    [ModelBinder(typeof(ModelBinder))]
    public class FileUpload
    {
        public string FileName { get; set; }
        public Stream InputStream { get; set; }
        public bool IsChunk { get; set; }
        public bool IsLastChunk { get; set; }

        public class ModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var request = controllerContext.RequestContext.HttpContext.Request;

                string fileName = request.Files[0].FileName;
                Stream inputStream = request.Files[0].InputStream;
                bool partial = request.Headers["Content-Range"] != null;
                bool lastChunk;
                if (partial)
                {
                    string[] fileInfo = request.Headers["Content-Range"].Split('/', '-');
                    lastChunk = Int32.Parse(fileInfo[1].Trim()) == Int32.Parse(fileInfo[2].Trim()) - 1;
                }
                else
                {
                    lastChunk = false;
                }

                return new FileUpload()
                {
                    FileName = fileName,
                    InputStream = inputStream,
                    IsChunk = partial,
                    IsLastChunk = lastChunk
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
