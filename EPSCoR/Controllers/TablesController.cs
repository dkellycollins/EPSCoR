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
using Newtonsoft.Json.Linq;
using WebMatrix.WebData;

namespace EPSCoR.Controllers
{
    [Authorize]
    public class TablesController : BootstrapBaseController
    {
        private IModelRepository<TableIndex> _tableIndexRepo;
        private ITableRepository _tableRepo;
        private IDatabaseCalc _dbCalc;

        public TablesController()
        {
            _tableIndexRepo = new BasicModelRepo<TableIndex>();
            _tableRepo = new BasicTableRepo(WebSecurity.CurrentUserName);
            _dbCalc = (IDatabaseCalc)_tableRepo;
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

        //
        // GET: /Tables/
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

        //
        // GET: /Tables/Details/{Table.ID}
        public ActionResult Details(string id)
        {
            DataTable table = _tableRepo.Read(id);
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

        //
        // POST: /Table/Delete/{Table.ID}
        [HttpPost]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            _tableRepo.Drop(id);
            DisplaySuccess(id + " deleted.");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Calc(FormCollection formCollection)
        {
            string attTable = formCollection["attTable"];
            string usTable = formCollection["usTable"];
            string calc = formCollection["calc"];

            CalcResult result = CalcResult.Unknown;
            switch (calc)
            {
                case "Sum":
                    result = _dbCalc.SumTables(attTable, usTable);
                    break;
                case "Avg":
                    result = _dbCalc.AvgTables(attTable, usTable);
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
                    DisplayInformation("The request has been submitted for processing.");
                    break;
            }
            return RedirectToAction("Index");
        }

        #region Helpers

        #endregion Helpers
    }
}
