using System.Web.Mvc;

namespace EPSCoR.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
