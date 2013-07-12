using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BootstrapSupport;
using EPSCoR.Database.Models;
using EPSCoR.Extensions;
using EPSCoR.Hubs;
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
            return RedirectToAction("Index", "Home");
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
                    TableHub.SendAlert("Calc table generated", WebSecurity.CurrentUserName, "Success!", Alerts.SUCCESS);
                    break;
                case CalcResult.Error:
                    TableHub.SendAlert("The server encountered an error while processing you request.", WebSecurity.CurrentUserName, "Error!", Alerts.ERROR);
                    break;
                case CalcResult.TableAlreadyExists:
                    TableHub.SendAlert("The calc table already exists. Please delete existing table before createing a new one.", WebSecurity.CurrentUserName, "Error!", Alerts.ERROR);
                    break;
                case CalcResult.SubmittedForProcessing:
                    TableHub.SendAlert("The request has been submitted for processing.", WebSecurity.CurrentUserName, "Success!", Alerts.SUCCESS);
                    break;
            }

            return RedirectToAction("Index", "Home");
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

        public ActionResult GetAllDetails()
        {
            IEnumerable<TableIndex> allDetails = _tableIndexRepo.GetAll().ToList();
            return new EPSCoR.Results.JsonResult(allDetails);
        }
    }
}
