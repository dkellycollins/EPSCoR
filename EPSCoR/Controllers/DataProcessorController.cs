using System.Web.Mvc;

namespace EPSCoR.Controllers
{
    /// <summary>
    /// Returns views for the data processor app.
    /// </summary>
    [Authorize]
    public class DataProcessorController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
