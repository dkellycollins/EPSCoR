using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPSCoR.Models;
using EPSCoR.Repositories;

namespace EPSCoR.Controllers
{
    [Authorize]
    public class TablesController : Controller
    {
        private IRepository<Table> _tableRepo;

        public TablesController()
        {
            _tableRepo = new BasicRepo<Table>();
        }

        public TablesController(IRepository<Table> repo)
        {
            _tableRepo = repo;
        }

        //
        // GET: /Tables/
        public ActionResult Index()
        {
            var tables = from t in _tableRepo.GetAll() select t;
            //This will not work as we are comparing UserProfile to IPrinciple. But I dont know what I should do instead.
            if(!this.User.IsInRole("admin"))
                tables = tables.Where((t) => t.User == this.User);

            return View(tables.ToList());
        }

        //
        // GET: /Tables/Details/{Table.ID}
        public ActionResult Details(int id = 0)
        {
            Table table = _tableRepo.Get(id);
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
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                Table newTable = new Table();
                newTable.Name = FileConverter.SaveFile(this.User.Identity.Name, file);
                _tableRepo.Create(newTable);
                ViewBag.UploadResult = "Upload Sucessful!";
            }
            catch(Exception e)
            {
                ViewBag.UploadResult = "Upload failed: " + e.Message;
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
            _tableRepo.Dispose();
            base.Dispose(disposing);
        }
    }
}
