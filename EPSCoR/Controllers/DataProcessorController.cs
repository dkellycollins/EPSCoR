using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
