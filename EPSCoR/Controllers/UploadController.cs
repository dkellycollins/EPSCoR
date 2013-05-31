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
            string fileName = Path.GetFileNameWithoutExtension(file.FileName);

            //Save the chunk.
            FileStreamWrapper wrapper = new FileStreamWrapper()
            {
                FileName = fileName + file.PartIndex,
                InputStream = file.InputStream
            };
            bool saveSuccessful = _tempFileAccessor.SaveFiles(wrapper);

            //If this was the last chunk, save the complete file.
            if (saveSuccessful && file.PartIndex == file.TotalParts - 1)
            {
                //Check to see if this table already exis
                TableIndex existingTable = _tableIndexRepo.GetAll().Where((t) => t.Name == fileName).FirstOrDefault();
                if (existingTable != null)
                {
                    //return new FineUploaderResult(false, error: "Table already exist. Remove existing table before uploading the new one.");
                }

                saveSuccessful = mergeTempFiles(file.FileName, file.TotalParts);
                deleteTempFiles(fileName, file.TotalParts);
            }
            
            //return new FineUploaderResult(saveSuccessful);
            return null;
        }

        public ActionResult DownloadCsv(string id)
        {
            string fileName = id + ".csv";
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
        public int PartIndex { get; set; }
        public int TotalParts { get; set; }

        public class ModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var request = controllerContext.RequestContext.HttpContext.Request;
                var formUpload = request.Files.Count > 0;

                return new FileUpload()
                {
                    InputStream = request.Files[0].InputStream
                };
            }
        }
    }

    public class FileUploadResult : ActionResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
