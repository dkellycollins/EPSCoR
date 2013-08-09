using System.Web.Mvc;

namespace EPSCoR.Controllers
{
    /// <summary>
    /// Serves as the entry point for the site.
    /// </summary>
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "DataProcessor");
        }
    }
}
