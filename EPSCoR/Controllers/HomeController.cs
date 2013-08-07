using System.Web.Mvc;

namespace EPSCoR.Controllers
{
    /// <summary>
    /// Serves as the entry point for the site.
    /// </summary>
    public class HomeController : Controller
    {
        //[OutputCache(Duration=0)]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "DataProcessor");
            else
                return RedirectToAction("Login", "Account");
        }
    }
}
