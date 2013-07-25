using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Factory;

namespace EPSCoR.Controllers
{
    [Authorize(Roles="Admin")]
    public class TaskController : Controller
    {
        public ActionResult Index()
        {
            var tasks = MvcApplication.FileProcessor.CurrentTasks.ToList();

            if (Request.IsAjaxRequest())
                return PartialView(tasks);
            return View(tasks);
        }

        [HttpPost]
        public ActionResult KillTask(int id)
        {
            if (MvcApplication.FileProcessor.CancelTokens.ContainsKey(id))
            {
                MvcApplication.FileProcessor.CancelTokens[id].Cancel();
                return RedirectToAction("Tasks");
            }
            return HttpNotFound();
        }
    }
}
