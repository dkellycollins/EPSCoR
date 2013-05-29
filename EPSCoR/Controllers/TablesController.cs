using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private IRepository<TablePairIndex> _tablePairIndexRepo;
        private IRepository<TableIndex> _tableIndexRepo;
        private IFileAccessor _uploadFileAccessor;
        private IFileAccessor _conversionFileAccessor;

        public TablesController()
        {
            _tablePairIndexRepo = new BasicRepo<TablePairIndex>();
            _userProfileRepo = new BasicRepo<UserProfile>();
            _tableIndexRepo = new BasicRepo<TableIndex>();
            _uploadFileAccessor = new BasicFileAccessor(BasicFileAccessor.UPLOAD_DIRECTORY, WebSecurity.CurrentUserName);
            _conversionFileAccessor = new BasicFileAccessor(BasicFileAccessor.CONVERTION_DIRECTORY, WebSecurity.CurrentUserName);
        }

        public TablesController(
            IRepository<TableIndex> tableIndexRepo,
            IRepository<TablePairIndex> tablePairIndexRepo, 
            IRepository<UserProfile> userProfileRepo, 
            IFileAccessor uploadFileAccessor, 
            IFileAccessor conversionFileAccessor)
        {
            _tableIndexRepo = tableIndexRepo;
            _tablePairIndexRepo = tablePairIndexRepo;
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

        //
        // GET: /Tables/Details/{Table.ID}
        public ActionResult Details(int id = 0)
        {
            TablePairIndex table = _tablePairIndexRepo.Get(id);
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

        //
        // POST: /Table/Upload/
        [HttpPost]
        public ActionResult fUpload(FineUpload file)
        {
            string tableName = Path.GetFileNameWithoutExtension(file.FileName);
            TableIndex existingTable = _tableIndexRepo.GetAll().Where((t) => t.Name == tableName).FirstOrDefault();
            if (existingTable != null)
            {
                //DisplayAttention("Table already exist. Remove existing table before uploading the new one.");
                //return RedirectToAction("Upload");
                return new FineUploaderResult(false, error: "Table already exist. Remove existing table before uploading the new one.");
            }

            //UserProfile userProfile = _userProfileRepo.GetAll().Where((u) => u.UserName == WebSecurity.CurrentUserName).FirstOrDefault();
            bool saveSuccessful = _uploadFileAccessor.SaveFiles(FileStreamWrapper.FromFineUpload(file));

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
            _tablePairIndexRepo.Dispose();
            _userProfileRepo.Dispose();
            base.Dispose(disposing);
        }
    }
}
