using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPSCoR.Controllers
{
    /// <summary>
    /// Contains public info pages such as the welcome page.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Returns the homepage.
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 0)]
        public ActionResult Index()
        {
            return View();
        }
    }
}
