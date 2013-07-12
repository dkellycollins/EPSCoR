using System.Linq;
using System.Web.Mvc;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Factory;
using WebMatrix.WebData;

namespace EPSCoR.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (IModelRepository<TableIndex> repo = RepositoryFactory.GetModelRepository<TableIndex>())
            {
                return View(
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

                if (Request.IsAjaxRequest()) //Handle ajax request.
                    return PartialView("_calcForm");
                else //Handle all other request.
                    return View("_calcForm");
            }
        }

        public ActionResult UploadForm()
        {
            if (Request.IsAjaxRequest()) //Handle ajax request.
                return PartialView("_uploadForm");
            else //Handle all other request.
                return View("_uploadForm");
        }

        public ActionResult AboutForm()
        {
            if (Request.IsAjaxRequest()) //Handle ajax request.
                return PartialView("_aboutForm");
            else //Handle all other request.
                return View("_aboutForm");
        }
    }
}
