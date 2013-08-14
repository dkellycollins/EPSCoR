using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web.Mvc;
using EPSCoR.Web.App.Filters;
using EPSCoR.Web.App.Repositories;
using EPSCoR.Web.App.Repositories.Factory;
using EPSCoR.Web.App.Results;
using EPSCoR.Web.App.ViewModels;
using EPSCoR.Web.Database.Models;
using WebMatrix.WebData;

namespace EPSCoR.Web.App.Controllers
{
    /// <summary>
    /// Provides functions for uploading and retriving files.
    /// </summary>
    [AddUserWhenAuthorized]
    public class FilesController : Controller
    {
        private IModelRepository<TableIndex> _tableIndexRepo;
        private IFileAccessor _fileAccessor;
        private IAsyncFileAccessor _asyncFileAccessor;

        public FilesController()
        {
            _tableIndexRepo = RepositoryFactory.GetModelRepository<TableIndex>();
            _fileAccessor = RepositoryFactory.GetFileAccessor(WebSecurity.CurrentUserName);
            _asyncFileAccessor = RepositoryFactory.GetAsyncFileAccessor(WebSecurity.CurrentUserName);
        }

        public FilesController(
            IModelRepository<TableIndex> tableIndexRepo,
            IFileAccessor fileAccessor,
            IAsyncFileAccessor asyncFileAccessor)
        {
            _tableIndexRepo = tableIndexRepo;
            _fileAccessor = fileAccessor;
            _asyncFileAccessor = asyncFileAccessor;
        }

        /// <summary>
        /// Handles saveing the posted file to the temp directory.
        /// </summary>
        /// <param name="file">Contains information on the file.</param>
        /// <returns>Status of the upload.</returns>
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

            bool saveSuccessful = _fileAccessor.SaveFiles(FileDirectory.Temp, wrapper);

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

            if (await _asyncFileAccessor.FileExistAsync(FileDirectory.Temp, id))
            {
                FileInfo info = await _asyncFileAccessor.GetFileInfoAsync(FileDirectory.Temp, id);
                uploadedBytes = (int)info.Length;
            }

            return new CheckFileResult(tableName, uploadedBytes, fileExists);
        }

        /// <summary>
        /// Finalizes the file upload.
        /// </summary>
        /// <param name="id">File to finalize.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CompleteUpload(string id)
        {
            if (!(await _asyncFileAccessor.FileExistAsync(FileDirectory.Temp, id)))
                return new FileUploadResult(id, "File has not been uploaded.");

            TableIndex index = new TableIndex()
            {
                Name = Path.GetFileNameWithoutExtension(id),
                UploadedByUser = WebSecurity.CurrentUserName,
                Type = (id.Contains("_US")) ? TableTypes.UPSTREAM : TableTypes.ATTRIBUTE,
                Status = "Queued for processing",
                FileKey = await _asyncFileAccessor.GenerateFileKeyAsync(FileDirectory.Temp, id)
            };
            _tableIndexRepo.Create(index);

            await _asyncFileAccessor.MoveFileAsync(FileDirectory.Temp, FileDirectory.Upload, id);

            return new FileUploadResult(id);
        }

        [HttpPost]
        [AddUserWhenAuthorized(Roles="Admin")]
        public async Task<ActionResult> CompleteUploadAdmin(string id, string tableName, string userName, string type)
        {
            if(!(await _asyncFileAccessor.FileExistAsync(FileDirectory.Temp, id)))
                return new FileUploadResult(id, "File has not been uploaded.");

            TableIndex existingTable = _tableIndexRepo.GetAll().Where((i) => i.Name == tableName && i.UploadedByUser == userName).FirstOrDefault();
            if(existingTable != null)
                return new FileUploadResult(id, "Table already exists");

            existingTable = new TableIndex()
            {
                Name = tableName,
                UploadedByUser = userName,
                Type = type,
                Status = "Queued for processing",
                FileKey = await _asyncFileAccessor.GenerateFileKeyAsync(FileDirectory.Temp, id)
            };
            _tableIndexRepo.Create(existingTable);

            await _asyncFileAccessor.MoveFileAsync(FileDirectory.Temp, FileDirectory.Upload, id);
            return new FileUploadResult(id);
        }

        /// <summary>
        /// Download a csv file.
        /// </summary>
        /// <param name="id">Name of the file to download.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> DownloadCsv(string id)
        {
            string fileName = id + ".csv";
            if(!(await _asyncFileAccessor.FileExistAsync(FileDirectory.Conversion, fileName)))
            {
                return new HttpNotFoundResult();
            }

            var cd = new ContentDisposition
            {
                FileName = fileName,
                Inline = false
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File((await _asyncFileAccessor.OpenFileAsync(FileDirectory.Conversion, fileName)), "text/csv");
        }
    }
}
