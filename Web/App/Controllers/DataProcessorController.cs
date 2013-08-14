using System.Web.Mvc;
using EPSCoR.Web.App.Filters;

namespace EPSCoR.Web.App.Controllers
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
