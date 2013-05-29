using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using BootstrapSupport;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using EPSCoR.ViewModels;
using Newtonsoft.Json.Linq;
using WebMatrix.WebData;

namespace EPSCoR.Controllers
{
    //[Authorize]
    public class TablesController : BootstrapBaseController
    {
        private IRepository<UserProfile> _userProfileRepo;
        private IRepository<TableIndex> _tableIndexRepo;
        private IFileAccessor _uploadFileAccessor;
        private IFileAccessor _conversionFileAccessor;
        private IFileAccessor _tempFileAccessor;

        public TablesController()
        {
            _userProfileRepo = new BasicRepo<UserProfile>();
            _tableIndexRepo = new BasicRepo<TableIndex>();
            _uploadFileAccessor = new BasicFileAccessor(BasicFileAccessor.UPLOAD_DIRECTORY, WebSecurity.CurrentUserName);
            _conversionFileAccessor = new BasicFileAccessor(BasicFileAccessor.CONVERTION_DIRECTORY, WebSecurity.CurrentUserName);
            _tempFileAccessor = new BasicFileAccessor(BasicFileAccessor.TEMP_DIRECTORY, WebSecurity.CurrentUserName);
        }

        public TablesController(
            IRepository<TableIndex> tableIndexRepo,
            IRepository<TablePairIndex> tablePairIndexRepo, 
            IRepository<UserProfile> userProfileRepo, 
            IFileAccessor uploadFileAccessor, 
            IFileAccessor conversionFileAccessor)
        {
            _tableIndexRepo = tableIndexRepo;
            _userProfileRepo = userProfileRepo;
            _uploadFileAccessor = uploadFileAccessor;
            _conversionFileAccessor = conversionFileAccessor;
        }

        //
        // GET: /Tables/
        public ActionResult Index()
        {
            var tables = from t in _tableIndexRepo.GetAll() select t;
            //UserProfile userProfile = _userProfileRepo.GetAll().Where((u) => u.UserName == WebSecurity.CurrentUserName).FirstOrDefault();
            //if(!this.User.IsInRole("admin"))
                //tables = tables.Where((t) => t. == this.User);

            return View(createTableIndexViewModel(tables.ToList()));
        }

        //
        // GET: /Tables/Details/{Table.ID}
        public ActionResult Details(int id = 0)
        {
            TableIndex table = _tableIndexRepo.Get(id);
            if (table == null)
            {
                return new HttpNotFoundResult();
            }
            return View(table);
        }

        //
        // GET: /Table/Upload/
        public ActionResult Upload()
        {
            List<string> fileNames = new List<string>();
            foreach(string fullFilePath in _uploadFileAccessor.GetFiles())
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

            return RedirectToAction("Upload");
        }

        private Dictionary<string, int> _chunksUploaded = new Dictionary<string, int>();

        //
        // POST: /Table/Upload/
        [HttpPost]
        public ActionResult fUpload(FineUpload file)
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
                    return new FineUploaderResult(false, error: "Table already exist. Remove existing table before uploading the new one.");
                }

                saveSuccessful = mergeTempFiles(file.FileName, file.TotalParts);
                deleteTempFiles(fileName, file.TotalParts);
            }
            
            return new FineUploaderResult(saveSuccessful);
        }

        //
        // GET: /Table/Delete/{Table.ID}
        public ActionResult Delete()
        {
            return View();
        }

        //
        // POST: /Table/Delete/{Table.ID}
        [HttpPost]
        public ActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            _userProfileRepo.Dispose();
            base.Dispose(disposing);
        }

        #region Helpers

        private TableIndexVM createTableIndexViewModel(List<TableIndex> tableIndexes)
        {
            TableIndexVM vm = new TableIndexVM();
            vm.Tables = new List<string>();
            vm.CalcForm = new CalcFormVM();
            vm.CalcForm.AttributeTables = new List<string>();
            vm.CalcForm.UpstreamTables = new List<string>();
            foreach (TableIndex index in tableIndexes)
            {
                vm.Tables.Add(index.Name);
                if (index.Type == TableTypes.ATTRIBUTE)
                    vm.CalcForm.AttributeTables.Add(index.Name);
                else if (index.Type == TableTypes.UPSTREAM)
                    vm.CalcForm.UpstreamTables.Add(index.Name);
            }

            vm.ConvertedTables = new List<ConvertedTablesVM>();
            foreach (string convertedFile in _conversionFileAccessor.GetFiles())
            {
                vm.ConvertedTables.Add(new ConvertedTablesVM()
                {
                    TableID = 0,
                    Table = Path.GetFileNameWithoutExtension(convertedFile),
                    Issuer = "",
                    Time = "",
                    Type = ""
                });
            }

            return vm;
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
                while (tempFileStream.Read(b, 0, b.Length) != 0)
                    completeFileStream.Write(b, 0, b.Length);
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

        #endregion Helpers
    }
}
