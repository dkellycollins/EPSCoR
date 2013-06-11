using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPSCoR.Controllers
{
    /// <summary>
    /// Contains private info pages such as the How to pages.
    /// </summary>
    [Authorize]
    public class AboutController : Controller
    {
        //
        // GET: /About/
        [OutputCache(Duration=0)]
        public ActionResult Index()
        {
            return HowToUpload();
        }

        //
        // GET: /About/HowToExport/
        [OutputCache(Duration = 0)]
        public ActionResult HowToExport()
        {
            return View();
        }

        //
        // GET: /About/HowToUpload/
        [OutputCache(Duration = 0)]
        public ActionResult HowToUpload()
        {
            return View();
        }
    }
}
