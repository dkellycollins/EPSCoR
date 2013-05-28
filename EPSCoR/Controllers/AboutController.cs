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
    //[Authorize]
    public class AboutController : Controller
    {
        //
        // GET: /About/
        public ActionResult Index()
        {
            return HowToUpload();
        }

        //
        // GET: /About/HowToExport/
        public ActionResult HowToExport()
        {
            return View();
        }

        //
        // GET: /About/HowToUpload/
        public ActionResult HowToUpload()
        {
            return View();
        }
    }
}
