using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using BootstrapSupport;
using EPSCoR.Database.Models;
using EPSCoR.Extensions;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Basic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebMatrix.WebData;
using EPSCoR.ViewModels;
using EPSCoR.Results;
using EPSCoR.Repositories.Factory;
using System.Threading.Tasks;

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

            var allTables = tables.Where(t => t.Processed);
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
        public async Task<ActionResult> Calc(CalcRequest calcRequest)
        {
            CalcResult result = CalcResult.Unknown;
            switch (calcRequest.CalcType)
            {
                case "Sum":
                    result = await Task.Factory.StartNew(() => _dbCalc.SumTables(calcRequest.AttributeTable, calcRequest.UpstreamTable));
                    break;
                case "Avg":
                    result = await Task.Factory.StartNew(() => _dbCalc.AvgTables(calcRequest.AttributeTable, calcRequest.UpstreamTable));
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
            IQueryable<TableIndex> tableIndexes = _tableIndexRepo.GetAll();
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
            DataTable data = _tableRepo.Read(args.TableName, args.DisplayStart, args.DisplayLength);
            int totalRows = _tableRepo.Count(args.TableName);
            int echo = Int32.Parse(args.Echo);

            return new DataTableResult(totalRows, totalRows, echo, data);
        }
    }
}
