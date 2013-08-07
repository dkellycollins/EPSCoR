using System.Web.Mvc;

namespace EPSCoR.Controllers
{
    /// <summary>
    /// Returns views for the data processor app.
    /// </summary>
    [Authorize]
    public class DataProcessorController : Controller
    {
        //[OutputCache(Duration=0)]
        public ActionResult Index()
        {
            return View();
        }

    }
}
