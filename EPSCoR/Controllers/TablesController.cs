using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using EPSCoR.ViewModels;
using WebMatrix.WebData;

namespace EPSCoR.Controllers
{
    //[Authorize]
    public class TablesController : Controller
    {
        private IRepository<UserProfile> _userProfileRepo;
        private IRepository<TableIndex> _tableIndexRepo;
        private IFileAccessor _uploadFileAccessor;

        public TablesController()
        {
            _tableIndexRepo = new BasicRepo<TableIndex>();
            _userProfileRepo = new BasicRepo<UserProfile>();
            _uploadFileAccessor = new BasicFileAccessor(BasicFileAccessor.UPLOAD_DIRECTORY, WebSecurity.CurrentUserName);
        }

        public TablesController(IRepository<TableIndex> tableIndexRepo, IRepository<UserProfile> userProfileRepo, IFileAccessor uploadFileAccessor)
        {
            _tableIndexRepo = tableIndexRepo;
            _userProfileRepo = userProfileRepo;
            _uploadFileAccessor = uploadFileAccessor;
        }

        //
        // GET: /Tables/
        public ActionResult Index()
        {
            var tables = from t in _tableIndexRepo.GetAll() select t;
            //if(!this.User.IsInRole("admin"))
                //tables = tables.Where((t) => t.User == this.User);

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
                vm.Tables.Add(index.AttributeTable);
                vm.Tables.Add(index.UpstreamTable);
                vm.CalcForm.AttributeTables.Add(index.AttributeTable);
                vm.CalcForm.UpstreamTables.Add(index.UpstreamTable);
            }

            vm.ConvertedTables = new List<ConvertedTablesVM>();

            return vm;
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
            return View();
        }

        //
        // POST: /Table/Upload/
        [HttpPost]
        public ActionResult Upload(TableIndex table, HttpPostedFileBase attFile, HttpPostedFileBase usFile)
        {
            TableIndex existingTable = _tableIndexRepo.GetAll().Where((t) => t.Name == table.Name && t.Version == table.Version).FirstOrDefault();
            if (existingTable != null)
            {
                TempData["StatusMessage"] = "Tables already exist";
                return RedirectToAction("Upload");
            }

            //UserProfile userProfile = _userProfileRepo.GetAll().Where((u) => u.UserName == WebSecurity.CurrentUserName).FirstOrDefault();
            bool saveSuccessful = _uploadFileAccessor.SaveFiles(attFile, usFile);

            if (saveSuccessful)
            {
                table.AttributeTable = Path.GetFileNameWithoutExtension(attFile.FileName);
                table.UpstreamTable = Path.GetFileNameWithoutExtension(usFile.FileName);

                _tableIndexRepo.Create(table);
                TempData["StatusMessage"] = "Upload Sucessful!";
            }
            else
            {
                TempData["StatusMessage"] = "Upload Failed.";
            }

            return RedirectToAction("Index");
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
            _tableIndexRepo.Dispose();
            _userProfileRepo.Dispose();
            base.Dispose(disposing);
        }
    }
}
