using System.Web.Mvc;
using EPSCoR.Filters;

namespace EPSCoR.Controllers
{
    /// <summary>
    /// Returns views for the data processor app.
    /// </summary>
    [AddUserWhenAuthorized]
    public class DataProcessorController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
