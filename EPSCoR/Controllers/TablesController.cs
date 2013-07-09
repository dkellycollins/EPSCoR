using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using EPSCoR.Database.Models;
using EPSCoR.Extensions;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Factory;
using EPSCoR.Results;
using EPSCoR.ViewModels;
using WebMatrix.WebData;

namespace EPSCoR.Controllers
{
    /// <summary>
    /// Contains views that display and work with uploaded tables.
    /// </summary>
    [Authorize]
    public class TablesController : BootstrapBaseController
    {
        private IModelRepository<TableIndex> _tableIndexRepo;
        private ITableRepository _tableRepo;
        private IDatabaseCalc _dbCalc;

        public TablesController()
        {
            _tableIndexRepo = RepositoryFactory.GetModelRepository<TableIndex>();
            _tableRepo = RepositoryFactory.GetTableRepository(WebSecurity.CurrentUserName);
            _dbCalc = RepositoryFactory.GetDatabaseCalc(WebSecurity.CurrentUserName);
        }

        public TablesController(
            IModelRepository<TableIndex> tableIndexRepo,
            ITableRepository tableRepo,
            IDatabaseCalc dbCalc)
        {
            _tableIndexRepo = tableIndexRepo;
            _tableRepo = tableRepo;
            _dbCalc = dbCalc;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _tableIndexRepo.Dispose();
            _tableRepo.Dispose();
            _dbCalc.Dispose();

            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        /// Returns a view that can display uploaded tables.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var tables = from t in _tableIndexRepo.GetAll() select t;

            var allTables = tables.Where(t => t.Processed && t.UploadedByUser == WebSecurity.CurrentUserName);
            var attTables = allTables.Where(t => t.Type == TableTypes.ATTRIBUTE);
            var usTables = allTables.Where(t => t.Type == TableTypes.UPSTREAM);

            ViewBag.AllTables = allTables.ToList();
            ViewBag.AttributeTables = attTables.ToList();
            ViewBag.UpstreamTables = usTables.ToList();

            return View();
        }

        /// <summary>
        /// Returns a view that displays the table.
        /// </summary>
        /// <param name="id">Name of the table.</param>
        /// <param name="lowerLimit">The lower limit to return.</param>
        /// <param name="upperLimit">The upper limit to return.</param>
        /// <returns></returns>
        public ActionResult Details(string id, int lowerLimit = 0, int upperLimit = 10)
        {
            //TODO convert to async method
            DataTable table = _tableRepo.Read(id, lowerLimit, upperLimit);
            if (table == null)
                return new HttpNotFoundResult();

            table.TableName = id;
            if (Request.IsJsonRequest()) //Handle json request.
                return Json(table);
            if (Request.IsAjaxRequest()) //Handle ajax request.
                return PartialView("DetailsPartial", table);
            else //Handle all other request.
                return View(table);
        }

        /// <summary>
        /// Deletes the table from the database.
        /// </summary>
        /// <param name="id">Name of the table to delete.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            _tableRepo.Drop(id);
            DisplaySuccess(id + " deleted.");
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Creates a calc table based on the form information provided.
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Calc(CalcRequest calcRequest)
        {
            //TODO convert to async method
            CalcResult result = CalcResult.Unknown;
            switch (calcRequest.CalcType)
            {
                case "Sum":
                    result = _dbCalc.SumTables(calcRequest.AttributeTable, calcRequest.UpstreamTable);
                    break;
                case "Avg":
                    result = _dbCalc.AvgTables(calcRequest.AttributeTable, calcRequest.UpstreamTable);
                    break;
            }

            switch (result)
            {
                case CalcResult.Success:
                    DisplaySuccess("Calc table generated");
                    break;
                case CalcResult.Error:
                    DisplayError("The server encountered an error while processing you request.");
                    break;
                case CalcResult.TableAlreadyExists:
                    DisplayError("The calc table already exists. Please delete existing table before createing a new one.");
                    break;
                case CalcResult.SubmittedForProcessing:
                    DisplaySuccess("The request has been submitted for processing.");
                    break;
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Returns a view listing all table status.
        /// </summary>
        /// <returns></returns>
        public ActionResult Status()
        {
            IQueryable<TableIndex> tableIndexes = _tableIndexRepo.GetAll().Where(t => t.UploadedByUser == WebSecurity.CurrentUserName);
            ViewBag.ProcessedTables = tableIndexes.Where((index) => index.Processed).ToList();
            ViewBag.NotProcessedTables = tableIndexes.Where((index) => !index.Processed).ToList();

            if (Request.IsAjaxRequest())
                return PartialView("StatusPartial");
            return View();
        }

        /// <summary>
        /// For use with datatable, return a portion of the table.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ActionResult DataTableDetails(DataTableParams args)
        {
            //TODO convert to async method
            DataTable data = _tableRepo.Read(args.TableName, args.DisplayStart, args.DisplayLength);
            int totalRows = _tableRepo.Count(args.TableName);
            int echo = Int32.Parse(args.Echo);

            return new DataTableResult(totalRows, totalRows, echo, data);
        }
    }
}
