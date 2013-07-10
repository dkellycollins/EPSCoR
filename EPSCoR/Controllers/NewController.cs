using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BootstrapSupport;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Factory;
using Microsoft.AspNet.SignalR;
using WebMatrix.WebData;

namespace EPSCoR.Controllers
{
    [System.Web.Mvc.Authorize]
    public class NewController : Controller
    {
        public ActionResult Index()
        {
            using (IModelRepository<TableIndex> repo = RepositoryFactory.GetModelRepository<TableIndex>())
            {
                return View(
                    "~/Views/2.0/Index.cshtml",
                    "~/Views/2.0/Layout2.0.cshtml",
                    repo.GetAll()
                        .Where(index => index.UploadedByUser == WebSecurity.CurrentUserName)
                        .ToList()
                );
            }
        }

        public ActionResult CalcForm()
        {
            using (IModelRepository<TableIndex> repo = RepositoryFactory.GetModelRepository<TableIndex>())
            {
                var tables = repo.GetAll();

                var allTables = tables.Where(t => t.Processed && t.UploadedByUser == WebSecurity.CurrentUserName);
                var attTables = allTables.Where(t => t.Type == TableTypes.ATTRIBUTE);
                var usTables = allTables.Where(t => t.Type == TableTypes.UPSTREAM);

                ViewBag.AllTables = allTables.ToList();
                ViewBag.AttributeTables = attTables.ToList();
                ViewBag.UpstreamTables = usTables.ToList();

                return View(
                    "~/Views/2.0/_calcForm.cshtml",
                    "~/Views/2.0/Layout2.0.cshtml",
                    null);
            }
        }

        public ActionResult UploadForm()
        {
            return View(
                "~/Views/2.0/_uploadForm.cshtml",
                "~/Views/2.0/Layout2.0.cshtml",
                null);
        }

        public ActionResult AboutForm()
        {
            return View(
                "~/Views/2.0/_aboutForm.cshtml",
                "~/Views/2.0/Layout2.0.cshtml",
                null);
        }
    }
}
