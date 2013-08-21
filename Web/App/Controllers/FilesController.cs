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
        private IRepositoryFactory _repoFactory;

        public FilesController()
        {
            _repoFactory = new RepositoryFactory();
        }

        public FilesController(IRepositoryFactory factory)
        {
            _repoFactory = factory;
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

            IFileAccessor fileAccessor = _repoFactory.GetFileAccessor(WebSecurity.CurrentUserName);
            bool saveSuccessful = fileAccessor.SaveFiles(FileDirectory.Temp, wrapper);

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

            using (IModelRepository<TableIndex> repo = _repoFactory.GetModelRepository<TableIndex>())
            {
                TableIndex existingTable = repo.Where(t => t.Name == tableName && t.UploadedByUser == userName).FirstOrDefault();
                fileExists = existingTable != null;
            }

            IAsyncFileAccessor asyncFileAccessor = _repoFactory.GetAsyncFileAccessor(WebSecurity.CurrentUserName);
            if (await asyncFileAccessor.FileExistAsync(FileDirectory.Temp, id))
            {
                FileInfo info = await asyncFileAccessor.GetFileInfoAsync(FileDirectory.Temp, id);
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
            IAsyncFileAccessor asyncFileAccessor = _repoFactory.GetAsyncFileAccessor(WebSecurity.CurrentUserName);
            if (!(await asyncFileAccessor.FileExistAsync(FileDirectory.Temp, id)))
                return new FileUploadResult(id, "File has not been uploaded.");

            TableIndex index = new TableIndex()
            {
                Name = Path.GetFileNameWithoutExtension(id),
                UploadedByUser = WebSecurity.CurrentUserName,
                Type = (id.Contains("_US")) ? TableTypes.UPSTREAM : TableTypes.ATTRIBUTE,
                Status = "Queued for processing",
                FileKey = await asyncFileAccessor.GenerateFileKeyAsync(FileDirectory.Temp, id)
            };
            using (IModelRepository<TableIndex> repo = _repoFactory.GetModelRepository<TableIndex>())
            {
                repo.Create(index);
            }

            await asyncFileAccessor.MoveFileAsync(FileDirectory.Temp, FileDirectory.Upload, id);
            return new FileUploadResult(id);
        }

        [HttpPost]
        [AddUserWhenAuthorized(Roles="Admin")]
        public async Task<ActionResult> CompleteUploadAdmin(string id, string tableName, string userName, string type)
        {
            IAsyncFileAccessor asyncFileAccessor = _repoFactory.GetAsyncFileAccessor(WebSecurity.CurrentUserName);
            if(!(await asyncFileAccessor.FileExistAsync(FileDirectory.Temp, id)))
                return new FileUploadResult(id, "File has not been uploaded.");

            using (IModelRepository<TableIndex> repo = _repoFactory.GetModelRepository<TableIndex>())
            {
                TableIndex existingTable = repo.Where((i) => i.Name == tableName && i.UploadedByUser == userName).FirstOrDefault();
                if (existingTable != null)
                    return new FileUploadResult(id, "Table already exists");

                existingTable = new TableIndex()
                {
                    Name = tableName,
                    UploadedByUser = userName,
                    Type = type,
                    Status = "Queued for processing",
                    FileKey = await asyncFileAccessor.GenerateFileKeyAsync(FileDirectory.Temp, id)
                };
                repo.Create(existingTable);
            }

            await asyncFileAccessor.MoveFileAsync(FileDirectory.Temp, FileDirectory.Upload, id);
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
            IAsyncFileAccessor asyncFileAccessor = _repoFactory.GetAsyncFileAccessor(WebSecurity.CurrentUserName);
            string fileName = id + ".csv";
            if(!(await asyncFileAccessor.FileExistAsync(FileDirectory.Conversion, fileName)))
            {
                return new HttpNotFoundResult();
            }

            var cd = new ContentDisposition
            {
                FileName = fileName,
                Inline = false
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File((await asyncFileAccessor.OpenFileAsync(FileDirectory.Conversion, fileName)), "text/csv");
        }
    }
}
