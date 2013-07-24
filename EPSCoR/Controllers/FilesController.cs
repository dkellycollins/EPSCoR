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
using EPSCoR.Filters;

namespace EPSCoR.Controllers
{
    /// <summary>
    /// Provides functions for uploading and retriving files.
    /// </summary>
    [EPSCoR.Filters.Authorize]
    public class FilesController : Controller
    {
        private IModelRepository<TableIndex> _tableIndexRepo;
        private IAsyncFileAccessor _fileAccessor;

        public FilesController()
        {
            _tableIndexRepo = RepositoryFactory.GetModelRepository<TableIndex>();
            _fileAccessor = RepositoryFactory.GetAsyncFileAccessor(WebSecurity.CurrentUserName);
        }

        public FilesController(
            IModelRepository<TableIndex> tableIndexRepo,
            IAsyncFileAccessor fileAccessor)
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
        /// Handles saveing the posted file to the temp directory.
        /// </summary>
        /// <param name="file">Contains information on the file.</param>
        /// <returns>Status of the upload.</returns>
        [HttpPost]
        public async Task<ActionResult> UploadFiles(FileUpload file)
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

            bool saveSuccessful = await _fileAccessor.SaveFilesAsync(FileDirectory.Temp, wrapper);

            if(saveSuccessful)
                return new FileUploadResult(fileName);
            return new FileUploadResult(fileName, "Could not save chunk.");
        }

        /// <summary>
        /// Returns json object containing info on the given file.
        /// </summary>
        /// <param name="id">Name of the file.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> CheckFile(string id)
        {
            int uploadedBytes = 0;
            bool fileExists = false;
            string tableName = Path.GetFileNameWithoutExtension(id);
            string userName = WebSecurity.CurrentUserName;

            var tables = _tableIndexRepo.GetAll();
            TableIndex existingTable = tables.Where(t => t.Name == tableName && t.UploadedByUser == userName).FirstOrDefault();
            fileExists = existingTable != null;

            if (await _fileAccessor.FileExistAsync(FileDirectory.Temp, id))
            {
                FileInfo info = await _fileAccessor.GetFileInfoAsync(FileDirectory.Temp, id);
                uploadedBytes = (int)info.Length;
            }

            return new NewtonsoftJsonResult(new 
            { 
                fileName = id, 
                uploadedBytes = uploadedBytes, 
                fileExists = fileExists 
            });
        }

        /// <summary>
        /// Finalizes the file upload.
        /// </summary>
        /// <param name="id">File to finalize.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CompleteUpload(string id)
        {
            if (!(await _fileAccessor.FileExistAsync(FileDirectory.Temp, id)))
                return new FileUploadResult(id, "File has not been uploaded.");

            bool result = false;
            using (FileStream fileStream = await _fileAccessor.OpenFileAsync(FileDirectory.Temp, id))
            {
                FileStreamWrapper wrapper = new FileStreamWrapper()
                {
                    FileName = id,
                    InputStream = fileStream
                };
                result = await _fileAccessor.SaveFilesAsync(FileDirectory.Upload, wrapper);
            }
            await _fileAccessor.DeleteFilesAsync(FileDirectory.Temp, id);

            if (result)
                return new FileUploadResult(id);
            return new FileUploadResult(id, "Could not complete file upload.");
        }

        /// <summary>
        /// Download a csv file.
        /// </summary>
        /// <param name="id">Name of the file to download.</param>
        /// <returns></returns>
        [HttpGet]
        //[OutputCache(VaryByParam="id", VaryByCustom="user")]
        public async Task<ActionResult> DownloadCsv(string id)
        {
            string fileName = id + ".csv";
            if(!(await _fileAccessor.FileExistAsync(FileDirectory.Conversion, fileName)))
            {
                return new HttpNotFoundResult();
            }

            var cd = new ContentDisposition
            {
                FileName = fileName,
                Inline = false
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File((await _fileAccessor.OpenFileAsync(FileDirectory.Conversion, fileName)), "text/csv");
        }
    }
}
