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
        }

        public TablesController(IRepository<TableIndex> repo)
        {
            _tableIndexRepo = repo;
            _uploadFileAccessor = new BasicFileAccessor(BasicFileAccessor.UPLOAD_DIRECTORY, WebSecurity.CurrentUserName);
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
        public ActionResult Upload(HttpPostedFileBase attFile, HttpPostedFileBase usFile)
        {
            TableIndex newTable = new TableIndex();

            UserProfile userProfile = _userProfileRepo.GetAll().Where((u) => u.UserName == WebSecurity.CurrentUserName).FirstOrDefault();
            bool attFileSaveResult = _uploadFileAccessor.SaveFile(attFile);
            bool usFileSaveResult = _uploadFileAccessor.SaveFile(usFile);

            if (attFileSaveResult && usFileSaveResult && userProfile != null)
            {
                newTable.AttributeTable = Path.GetFileNameWithoutExtension(attFile.FileName);
                newTable.UpstreamTable = Path.GetFileNameWithoutExtension(usFile.FileName);
                newTable.User = userProfile;
                newTable.Name = "";
                newTable.Region = "";
                newTable.Version = "";

                _tableIndexRepo.Create(newTable);
                ViewData["StatusMessage"] = "Upload Sucessful!";
            }
            else
            {
                ViewData["StatusMessage"] = "Upload Failed.";
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
            base.Dispose(disposing);
        }
    }
}
