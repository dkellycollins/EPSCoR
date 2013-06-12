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
using EPSCoR.Repositories;
using EPSCoR.Repositories.Basic;
using EPSCoR.ViewModels;
using Newtonsoft.Json.Linq;
using WebMatrix.WebData;

namespace EPSCoR.Controllers
{
    [Authorize]
    public class TablesController : BootstrapBaseController
    {
        private IModelRepository<UserProfile> _userProfileRepo;
        private IModelRepository<TableIndex> _tableIndexRepo;
        private IFileAccessor _conversionFileAccessor;
        private ITableRepository _tableRepo;
        private IDatabaseCalc _dbCalc;

        public TablesController()
        {
            _userProfileRepo = new BasicModelRepo<UserProfile>();
            _tableIndexRepo = new BasicModelRepo<TableIndex>();
            _conversionFileAccessor = BasicFileAccessor.GetConversionsAccessor(WebSecurity.CurrentUserName);
            _tableRepo = new BasicTableRepo(WebSecurity.CurrentUserName);
            _dbCalc = (IDatabaseCalc)_tableRepo;
        }

        public TablesController(
            IModelRepository<TableIndex> tableIndexRepo,
            IModelRepository<UserProfile> userProfileRepo)
        {
            _tableIndexRepo = tableIndexRepo;
            _userProfileRepo = userProfileRepo;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _tableIndexRepo.Dispose();
            _tableRepo.Dispose();
            _userProfileRepo.Dispose();

            base.OnActionExecuted(filterContext);
        }

        //
        // GET: /Tables/
        public ActionResult Index()
        {
            var tables = from t in _tableIndexRepo.GetAll() select t;
            //UserProfile userProfile = _userProfileRepo.GetAll().Where((u) => u.UserName == WebSecurity.CurrentUserName).FirstOrDefault();
            //if(!this.User.IsInRole("admin"))
                //tables = tables.Where((t) => t. == this.User);

            return View(createTableIndexViewModel(tables.ToList()));
        }

        //
        // GET: /Tables/Details/{Table.ID}
        public ActionResult Details(string id)
        {
            try
            {
                string userName = WebSecurity.CurrentUserName;
                TableIndex tIndex = _tableIndexRepo.GetAll().Where((t) => t.Name == id && t.UploadedByUser == userName).FirstOrDefault();
                DataTable table = _tableRepo.Read(tIndex.GetTableName());
                table.TableName = id;
                if (table == null)
                    return new HttpNotFoundResult();
                if (Request["format"] == "json") //Handle json request.
                    return Json(table);
                if (Request.IsAjaxRequest()) //Handle ajax request.
                    return PartialView(table);
                else //Handle all other request.
                    return View(table);
            }
            catch
            {
                return new HttpNotFoundResult();
            }
        }

        //
        // GET: /Table/Delete/{Table.ID}
        [HttpGet]
        public ActionResult Delete()
        {
            return View();
        }

        //
        // POST: /Table/Delete/{Table.ID}
        [HttpPost]
        public ActionResult Delete(string id)
        {
            string userName = WebSecurity.CurrentUserName;
            TableIndex tIndex = _tableIndexRepo.GetAll().Where((t) => t.Name == id && t.UploadedByUser == userName).FirstOrDefault();
            _tableRepo.Drop(tIndex.GetTableName());
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Calc(FormCollection formCollection)
        {
            string attTable = formCollection["attTable"];
            string usTable = formCollection["usTable"];
            string calc = formCollection["calc"];

            switch (calc)
            {
                case "Sum":
                    _dbCalc.SumTables(attTable, usTable);
                    break;
                case "Avg":
                    _dbCalc.AvgTables(attTable, usTable);
                    break;
            }

            DisplaySuccess("Calc table generated");
            return RedirectToAction("Index");
        }

        #region Helpers

        private TableIndexVM createTableIndexViewModel(List<TableIndex> tableIndexes)
        {
            TableIndexVM vm = new TableIndexVM();
            vm.Tables = new List<string>();
            vm.CalcForm = new CalcFormVM();
            vm.CalcForm.AttributeTables = new List<string>();
            vm.CalcForm.UpstreamTables = new List<string>();
            foreach (TableIndex index in tableIndexes)
            {
                if (!index.Processed)
                    continue; //Don't display any tables that cannot be used.

                vm.Tables.Add(index.Name);
                if (index.Type == TableTypes.ATTRIBUTE)
                    vm.CalcForm.AttributeTables.Add(index.Name);
                else if (index.Type == TableTypes.UPSTREAM)
                    vm.CalcForm.UpstreamTables.Add(index.Name);
            }

            return vm;
        }

        #endregion Helpers
    }
}
