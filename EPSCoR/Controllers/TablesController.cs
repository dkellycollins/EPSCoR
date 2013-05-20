using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using WebMatrix.WebData;

namespace EPSCoR.Controllers
{
    [Authorize]
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
            if(!this.User.IsInRole("admin"))
                tables = tables.Where((t) => t.User == this.User);

            return View(tables.ToList());
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

            UserProfile userProfile = _userProfileRepo.GetAll().Where((u) => u.UserName == WebSecurity.CurrentUserName).FirstOrDefault();
            bool saveSuccessful = _uploadFileAccessor.SaveFiles(attFile, usFile);

            if (saveSuccessful && userProfile != null)
            {
                table.AttributeTable = Path.GetFileNameWithoutExtension(attFile.FileName);
                table.UpstreamTable = Path.GetFileNameWithoutExtension(usFile.FileName);
                table.User = userProfile;

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
