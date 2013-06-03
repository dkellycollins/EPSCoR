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
    public class UploadController : BootstrapBaseController
    {
        private IRepository<TableIndex> _tableIndexRepo;
        private IFileAccessor _uploadFileAccessor;
        private IFileAccessor _conversionFileAccessor;
        private IFileAccessor _tempFileAccessor;

        public UploadController()
        {
            _tableIndexRepo = new BasicRepo<TableIndex>();
            _uploadFileAccessor = new BasicFileAccessor(BasicFileAccessor.UPLOAD_DIRECTORY, WebSecurity.CurrentUserName);
            _conversionFileAccessor = new BasicFileAccessor(BasicFileAccessor.CONVERTION_DIRECTORY, WebSecurity.CurrentUserName);
            _tempFileAccessor = new BasicFileAccessor(BasicFileAccessor.TEMP_DIRECTORY, WebSecurity.CurrentUserName);
        }

        public UploadController(
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

            //Circumventing the IFileAccesors for now.
            string uploadDirectory = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Uploads");
            string userDirecotry = Path.Combine(uploadDirectory, WebSecurity.CurrentUserName);
            string filePath = Path.Combine(userDirecotry, fileName);
            if (file.PartialFile)
            {
                try
                {
                    FileStream fileStream;
                    if(System.IO.File.Exists(filePath))
                        fileStream = System.IO.File.Open(filePath, FileMode.Append);
                    else
                        fileStream = System.IO.File.Create(filePath);

                    file.InputStream.CopyTo(fileStream);
                    fileStream.Flush();
                    fileStream.Close();

                    return new FileUploadResult(fileName, true);
                }
                catch(Exception e)
                {
                    return new FileUploadResult(fileName, false);
                }
            }
            else
            {
                try
                {
                    FileStream fileStream = System.IO.File.Create(filePath);
                    file.InputStream.CopyTo(fileStream);
                    fileStream.Flush();
                    fileStream.Close();

                    return new FileUploadResult(fileName, true);
                }
                catch (Exception e)
                {
                    return new FileUploadResult(fileName, false);
                }
            }
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
                MemoryStream tempFileStream = _tempFileAccessor.OpenFile(fileName + i);
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
        public bool PartialFile { get; set; }

        public class ModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var request = controllerContext.RequestContext.HttpContext.Request;

                bool partial = !string.IsNullOrEmpty(request.Headers["X-File-Name"]);
                string fileName;
                if(partial)
                    fileName = request.Headers["X-File-Name"];
                else
                    fileName = request.Files[0].FileName;
                Stream inputStream = request.Files[0].InputStream;

                return new FileUpload()
                {
                    FileName = fileName,
                    InputStream = inputStream,
                    PartialFile = partial
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
            public bool Success { get; set; }
        }

        private JavaScriptSerializer _serializer;
        private FileStatus _status;

        public FileUploadResult(FileStatus status)
        {
            _serializer = new JavaScriptSerializer();
            _status = status;
        }

        public FileUploadResult(string FileName, bool success)
        {
            _serializer = new JavaScriptSerializer();
            _status = new FileStatus()
            {
                Name = FileName,
                Success = success
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
