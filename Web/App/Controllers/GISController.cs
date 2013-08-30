using System.Web.Mvc;

namespace EPSCoR.Web.App.Controllers
{
    /// <summary>
    /// Returns views for the GIS interface.
    /// </summary>
    public class GISController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
