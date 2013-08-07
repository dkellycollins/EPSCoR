using System.Linq;
using System.Web.Mvc;

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
