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
        /// Returns the upload view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Upload()
        {
            return View();
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
            FileUploadResult result = new FileUploadResult();
            result.Name = id;

            _fileAccessor.CurrentDirectory = FileDirectory.Temp;
            if (_fileAccessor.FileExist(id))
            {
                FileInfo info = _fileAccessor.GetFileInfo(id);
                result.UploadedBytes = (int)info.Length;
            }

            return result;
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
            string tableName = Path.GetFileNameWithoutExtension(id);
            string userName = WebSecurity.CurrentUserName;
            TableIndex existingTable = _tableIndexRepo.GetAll().Where(t => t.Name == tableName && t.UploadedByUser == userName).FirstOrDefault();
            if (existingTable != null)
                return new FileUploadResult(id, "Table already exists. Delete exisiting table before uploading a new one.");
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
        /// Returns a view listing all files that can be downloaded.
        /// </summary>
        /// <returns></returns>
        public ActionResult Download()
        {
            _fileAccessor.CurrentDirectory = FileDirectory.Temp;
            var tableIndexes = _tableIndexRepo.GetAll().ToList();
            List<TableIndex> existingConversions = new List<TableIndex>();
            foreach (TableIndex tableIndex in tableIndexes)
            {
                if (_fileAccessor.FileExist(tableIndex.Name + ".csv"))
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
