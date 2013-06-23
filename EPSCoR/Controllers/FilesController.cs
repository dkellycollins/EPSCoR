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
using EPSCoR.Results;
using EPSCoR.ViewModels;

namespace EPSCoR.Controllers
{
    [Authorize]
    public class FilesController : BootstrapBaseController
    {
        private IModelRepository<TableIndex> _tableIndexRepo;
        private IFileAccessor _uploadFileAccessor;
        private IFileAccessor _conversionFileAccessor;
        private IFileAccessor _tempFileAccessor;

        public FilesController()
        {
            _tableIndexRepo = RepositoryFactory.GetModelRepository<TableIndex>();
            _uploadFileAccessor = RepositoryFactory.GetUploadFileAccessor(WebSecurity.CurrentUserName);
            _conversionFileAccessor = RepositoryFactory.GetConvertionFileAccessor(WebSecurity.CurrentUserName);
            _tempFileAccessor = RepositoryFactory.GetTempFileAccessor(WebSecurity.CurrentUserName);
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

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _tableIndexRepo.Dispose();

            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        /// Returns the upload view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Upload()
        {
            List<string> fileNames = new List<string>();
            foreach (string fullFilePath in _uploadFileAccessor.GetFiles())
                fileNames.Add(Path.GetFileName(fullFilePath));
            return View(fileNames);
        }

        /// <summary>
        /// The post method for a "simple" file upload. This expects the file to be uploaded through a basic form.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
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

        /// <summary>
        /// The post method for the jQueryFileUpload
        /// </summary>
        /// <param name="file">Contains information on the file.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFiles(FileUpload file)
        {
            string fileName = Path.GetFileName(file.FileName);

            //Save the chunk.
            FileStreamWrapper wrapper = new FileStreamWrapper()
            {
                FileName = fileName,
                InputStream = file.InputStream,
                SeekPos = file.StartPosition,
                FileSize = file.TotalFileLength
            };

            bool saveSuccessful = _tempFileAccessor.SaveFiles(wrapper);

            if(saveSuccessful)
                return new FileUploadResult(fileName);
            return new FileUploadResult(fileName, "Could not save chunk.");
        }

        /// <summary>
        /// Finalizes the file upload.
        /// </summary>
        /// <param name="id">File to finalize.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CompleteUpload(string id)
        {
            string tableName = Path.GetFileNameWithoutExtension(id);
            string userName = WebSecurity.CurrentUserName;
            TableIndex existingTable = _tableIndexRepo.GetAll().Where(t => t.Name == tableName && t.UploadedByUser == userName).FirstOrDefault();
            if (existingTable != null)
                return new FileUploadResult(id, "Table already exists. Delete exisiting table before uploading a new one.");
            if (!_tempFileAccessor.FileExist(id))
                return new FileUploadResult(id, "File has not been uploaded.");

            bool result = false;
            using(FileStream fileStream = _tempFileAccessor.OpenFile(id))
            {
                FileStreamWrapper wrapper = new FileStreamWrapper()
                {
                    FileName = id,
                    InputStream = fileStream
                };
                result = _uploadFileAccessor.SaveFiles(wrapper);
            }
            _tempFileAccessor.DeleteFiles(id);

            if (result)
                return new FileUploadResult(id);
            return new FileUploadResult(id, "Could not complete file upload.");
        }

        /// <summary>
        /// Returns a view listing all files that can be downloaded.
        /// </summary>
        /// <returns></returns>
        public ActionResult Download()
        {
            var tableIndexes = _tableIndexRepo.GetAll().ToList();
            List<TableIndex> existingConversions = new List<TableIndex>();
            foreach (TableIndex tableIndex in tableIndexes)
            {
                if (_conversionFileAccessor.FileExist(tableIndex.Name + ".csv"))
                    existingConversions.Add(tableIndex);
            }
            return View(existingConversions);
        }

        /// <summary>
        /// Download a csv file.
        /// </summary>
        /// <param name="id">Name of the file to download.</param>
        /// <returns></returns>
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
    }
}
