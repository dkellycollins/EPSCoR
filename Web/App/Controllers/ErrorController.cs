using System.Web.Mvc;

namespace EPSCoR.Web.App.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
