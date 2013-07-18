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
using EPSCoR.Repositories.Factory;
using System.Threading.Tasks;

namespace EPSCoR.Controllers
{
    [Authorize]
    public class FilesController : BootstrapBaseController
    {
        private IModelRepository<TableIndex> _tableIndexRepo;
        private IFileAccessor _fileAccessor;

        public FilesController()
        {
            _tableIndexRepo = RepositoryFactory.GetModelRepository<TableIndex>();
            _fileAccessor = RepositoryFactory.GetFileAccessor(WebSecurity.CurrentUserName);
        }

        public FilesController(
            IModelRepository<TableIndex> tableIndexRepo,
            IFileAccessor fileAccessor)
        {
            _tableIndexRepo = tableIndexRepo;
            _fileAccessor = fileAccessor;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _tableIndexRepo.Dispose();

            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        /// The post method for a "simple" file upload. This expects the file to be uploaded through a basic form.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {
            string userName = WebSecurity.CurrentUserName;
            string tableName = Path.GetFileNameWithoutExtension(file.FileName);
            TableIndex existingTable = _tableIndexRepo.GetAll().Where(t => t.Name == tableName && t.UploadedByUser == userName).FirstOrDefault();
            if (existingTable != null)
            {
                DisplayAttention("Table already exist. Remove existing table before uploading the new one.");
                return RedirectToAction("Upload");
            }

            _fileAccessor.CurrentDirectory = FileDirectory.Upload;
            bool saveSuccessfull = _fileAccessor.SaveFiles(FileStreamWrapper.FromHttpPostedFile(file));

            if (saveSuccessfull)
            {
                DisplaySuccess("Upload Successful!");
            }
            else
            {
                DisplayError("Upload failed.");
            }

            return new HttpStatusCodeResult(200);
        }

        /// <summary>
        /// The post method for the jQueryFileUpload
        /// </summary>
        /// <param name="file">Contains information on the file.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadFiles(FileUpload file)
        {
            //TODO convert to async method
            string fileName = Path.GetFileName(file.FileName);

            //Save the chunk.
            FileStreamWrapper wrapper = new FileStreamWrapper()
            {
                FileName = fileName,
                InputStream = file.InputStream,
                SeekPos = file.StartPosition,
                FileSize = file.TotalFileLength
            };

            _fileAccessor.CurrentDirectory = FileDirectory.Temp;
            bool saveSuccessful = _fileAccessor.SaveFiles(wrapper);

            if(saveSuccessful)
                return new FileUploadResult(fileName);
            return new FileUploadResult(fileName, "Could not save chunk.");
        }

        /// <summary>
        /// Returns file info if the file is stored in temp uploads.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CheckFile(string id)
        {
            //TODO convert to async method
            int uploadedBytes = 0;
            bool fileExists = false;
            string tableName = Path.GetFileNameWithoutExtension(id);
            string userName = WebSecurity.CurrentUserName;

            TableIndex existingTable = _tableIndexRepo.GetAll().Where(t => t.Name == tableName && t.UploadedByUser == userName).FirstOrDefault();
            fileExists = existingTable != null;

            _fileAccessor.CurrentDirectory = FileDirectory.Temp;
            if (_fileAccessor.FileExist(id))
            {
                FileInfo info = _fileAccessor.GetFileInfo(id);
                uploadedBytes = (int)info.Length;
            }

            return new EPSCoR.Results.JsonResult(new { fileName = id, uploadedBytes = uploadedBytes, fileExists = fileExists });
        }

        /// <summary>
        /// Finalizes the file upload.
        /// </summary>
        /// <param name="id">File to finalize.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CompleteUpload(string id)
        {
            //TODO convert to async method
            
            _fileAccessor.CurrentDirectory = FileDirectory.Temp;
            if (!_fileAccessor.FileExist(id))
                return new FileUploadResult(id, "File has not been uploaded.");

            bool result = false;
            using(FileStream fileStream = _fileAccessor.OpenFile(id))
            {
                FileStreamWrapper wrapper = new FileStreamWrapper()
                {
                    FileName = id,
                    InputStream = fileStream
                };
                _fileAccessor.CurrentDirectory = FileDirectory.Upload;
                result = _fileAccessor.SaveFiles(wrapper);
            }
            _fileAccessor.CurrentDirectory = FileDirectory.Temp;
            _fileAccessor.DeleteFiles(id);

            if (result)
                return new FileUploadResult(id);
            return new FileUploadResult(id, "Could not complete file upload.");
        }

        /// <summary>
        /// Download a csv file.
        /// </summary>
        /// <param name="id">Name of the file to download.</param>
        /// <returns></returns>
        public ActionResult DownloadCsv(string id)
        {
            _fileAccessor.CurrentDirectory = FileDirectory.Conversion;
            string fileName = id + ".csv";
            if(!_fileAccessor.FileExist(fileName))
            {
                return new HttpNotFoundResult();
            }

            var cd = new ContentDisposition
            {
                FileName = fileName,
                Inline = false
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(_fileAccessor.OpenFile(fileName), "text/csv");
        }
    }
}
