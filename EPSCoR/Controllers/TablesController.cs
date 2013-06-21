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
        public ActionResult Details(string id, int page = 0, int pageSize = 10)
        {
            int lowerLimit = page * pageSize;
            int upperLimit = lowerLimit + pageSize;
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
                    DisplaySuccess("The request has been submitted for processing.");
                    break;
            }
            return RedirectToAction("Index");
        }

        public ActionResult Status()
        {
            if (Request.IsAjaxRequest())
                return PartialView("StatusPartial", _tableIndexRepo.GetAll().ToList());
            return View(_tableIndexRepo.GetAll().ToList());
        }

        public ActionResult JsonDetails(DataTablesParams args)
        {
            DataTable data = _tableRepo.Read(args.TableName, args.DisplayStart, args.DisplayLength);
            int totalRows = _tableRepo.Count(args.TableName);
            int echo = Int32.Parse(args.Echo);

            return new DataTablesResult(totalRows, totalRows, echo, data);
        }

        #region Helpers

        #endregion Helpers
    }

    [ModelBinder(typeof(DataTablesParamsBinder))]
    public class DataTablesParams
    {
        /// <summary>
        /// Display start point in the current data set.
        /// </summary>
        public int DisplayStart { get; set; }

        /// <summary>
        /// Number of records that the table can display in the current draw. It is expected that the number of records returned will be equal to this number, unless the server has fewer records to return.
        /// </summary>
        public int DisplayLength { get; set; }

        /// <summary>
        /// Number of columns being displayed (useful for getting individual column search info)
        /// </summary>
        public int Columns { get; set; }

        /// <summary>
        /// Global search field
        /// </summary>
        public string GlobalSearch { get; set; }

        /// <summary>
        /// True if the global filter should be treated as a regular expression for advanced filtering, false if not.
        /// </summary>
        public bool GlobalRegex { get; set; }

        /// <summary>
        /// Indicator for if a column is flagged as searchable or not on the client-side
        /// </summary>
        public bool[] Searchable { get; set; }

        /// <summary>
        /// Individual column filter
        /// </summary>
        public string[] ColumnSearch { get; set; }

        /// <summary>
        /// True if the individual column filter should be treated as a regular expression for advanced filtering, false if not
        /// </summary>
        public bool[] ColumnRegex { get; set; }

        /// <summary>
        /// Indicator for if a column is flagged as sortable or not on the client-side
        /// </summary>
        public bool[] Sortable { get; set; }

        /// <summary>
        /// Number of columns to sort on
        /// </summary>
        public int SortingCols { get; set; }

        /// <summary>
        /// Column being sorted on (you will need to decode this number for your database)
        /// </summary>
        public int[] SortCol { get; set; }

        /// <summary>
        /// Direction to be sorted - "desc" or "asc".
        /// </summary>
        public string[] SortDir { get; set; }

        /// <summary>
        /// The value specified by mDataProp for each column. This can be useful for ensuring that the processing of data is independent from the order of the columns.
        /// </summary>
        public string[] DataProp { get; set; }

        /// <summary>
        /// Information for DataTables to use for rendering.
        /// </summary>
        public string Echo { get; set; }

        /// <summary>
        /// Custom parameter that contains the name of the table.
        /// </summary>
        public string TableName { get; set; }

        public class DataTablesParamsBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var request = controllerContext.HttpContext.Request;

                DataTablesParams result = new DataTablesParams();
                if(request["iDisplayStart"] != null)
                    result.DisplayStart = Int32.Parse(request["iDisplayStart"]);
                if(request["iDisplayLength"] != null)
                    result.DisplayLength = Int32.Parse(request["iDisplayLength"]);
                if(request["iColumns"] != null)
                    result.Columns = Int32.Parse(request["iColumns"]);
                if(request["sSearch"] != null)
                    result.GlobalSearch = request["sSearch"];
                if(request["bRegex"] != null)
                    result.GlobalRegex = Boolean.Parse(request["bRegex"]);
                if (request["bSearchable_0"] != null)
                {
                    result.Searchable = new bool[result.Columns];
                    for (int i = 0; i < result.Columns; i++)
                    {
                        result.Searchable[i] = Boolean.Parse(request["bSearchable_" + i]);
                    }
                }
                if (request["sSearch_0"] != null)
                {
                    result.ColumnSearch = new string[result.Columns];
                    for (int i = 0; i < result.Columns; i++)
                    {
                        result.ColumnSearch[i] = request["sSearch_" + i];
                    }
                }
                if (request["bRegex_0"] != null)
                {
                    result.ColumnRegex = new bool[result.Columns];
                    for (int i = 0; i < result.Columns; i++)
                    {
                        result.ColumnRegex[i] = Boolean.Parse(request["bRegex_" + i]);
                    }
                }
                if (request["bSortable_0"] != null)
                {
                    result.Sortable = new bool[result.Columns];
                    for (int i = 0; i < result.Columns; i++)
                    {
                        result.Sortable[i] = Boolean.Parse(request["bSortable_" + i]);
                    }
                }
                if(request["iSortingCols"] != null)
                    result.SortingCols = Int32.Parse(request["iSortingCols"]);
                if (request["iSortCol_0"] != null)
                {
                    result.SortCol = new int[result.SortingCols];
                    for (int i = 0; i < result.SortingCols; i++)
                    {
                        result.SortCol[i] = Int32.Parse(request["iSortCol_" + i]);
                    }
                }
                if (request["sSortDir_0"] != null)
                {
                    result.SortDir = new string[result.SortingCols];
                    for (int i = 0; i < result.SortingCols; i++)
                    {
                        result.SortDir[i] = request["sSortDir_" + i];
                    }
                }
                if (request["mDataProp_0"] != null)
                {
                    result.DataProp = new string[result.Columns];
                    for (int i = 0; i < result.Columns; i++)
                    {
                        result.DataProp[i] = request["mDataProp_" + i];
                    }
                }
                if(request["sEcho"] != null)
                    result.Echo = request["sEcho"];
                if(request["tableName"] != null)
                    result.TableName = request["tableName"];

                return result;
            }
        }
    }

    public class DataTablesResult : ActionResult
    {
        /// <summary>
        /// Total records, before filtering (i.e. the total number of records in the database)
        /// </summary>
        public int iTotalRecords { get; set; }

        /// <summary>
        /// Total records, after filtering (i.e. the total number of records after filtering has been applied - not just the number of records being returned in this result set)
        /// </summary>
        public int iTotalDisplayRecords { get; set; }

        /// <summary>
        /// An unaltered copy of sEcho sent from the client side. This parameter will change with each draw (it is basically a draw count) - so it is important that this is implemented. Note that it strongly recommended for security reasons that you 'cast' this parameter to an integer in order to prevent Cross Site Scripting (XSS) attacks.
        /// </summary>
        public string sEcho { get; set; }

        /// <summary>
        /// The data in a 2D array. Note that you can change the name of this parameter with sAjaxDataProp.
        /// </summary>
        public object[,] aaData { get; set; }

        public DataTablesResult(int totalRecords, int totalDisplayRecords, int echo, DataTable data)
        {
            iTotalRecords = totalRecords;
            iTotalDisplayRecords = totalDisplayRecords;
            sEcho = echo.ToString();
            int numCol = data.Columns.Count;
            int numRow = data.Rows.Count;
            aaData = new object[numRow, numCol];
            for (int i = 0; i < numRow; i++)
            {
                for (int j = 0; j < numCol; j++)
                {
                    aaData[i, j] = data.Rows[i][j];
                }
            }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            string json = JsonConvert.SerializeObject(this);

            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.Write(json);
        }
    }
}
