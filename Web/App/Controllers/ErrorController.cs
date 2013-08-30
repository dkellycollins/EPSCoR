using System.Web.Mvc;

namespace EPSCoR.Web.App.Controllers
{
    /// <summary>
    /// Returns custom error pages.
    /// </summary>
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
