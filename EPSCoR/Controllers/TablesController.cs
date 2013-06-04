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
using EPSCoR.ViewModels;
using Newtonsoft.Json.Linq;
using WebMatrix.WebData;

namespace EPSCoR.Controllers
{
    //[Authorize]
    public class TablesController : BootstrapBaseController
    {
        private IRepository<UserProfile> _userProfileRepo;
        private IRepository<TableIndex> _tableIndexRepo;
        private IFileAccessor _conversionFileAccessor;

        public TablesController()
        {
            _userProfileRepo = new BasicRepo<UserProfile>();
            _tableIndexRepo = new BasicRepo<TableIndex>();
            _conversionFileAccessor = BasicFileAccessor.GetConversionsAccessor(WebSecurity.CurrentUserName);
        }

        public TablesController(
            IRepository<TableIndex> tableIndexRepo,
            IRepository<UserProfile> userProfileRepo)
        {
            _tableIndexRepo = tableIndexRepo;
            _userProfileRepo = userProfileRepo;
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
            DefaultContext context = DefaultContext.GetInstance();
            try
            {
                //TODO abstract this into a repository.
                DataTable table = context.GetTable(id);
                if (Request.IsAjaxRequest())
                    return PartialView(table);
                else
                    return View(table);
            }
            catch
            {
                return new HttpNotFoundResult();
            }
            finally
            {
                DefaultContext.Release();
            }
        }

        //
        // GET: /Table/Delete/{Table.ID}
        public ActionResult Delete()
        {
            return View();
        }

        //
        // POST: /Table/Delete/{Table.ID}
        [HttpPost]
        public ActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            _userProfileRepo.Dispose();
            base.Dispose(disposing);
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
                    //Pereform sum.
                    break;
                case "Avg":
                    //Perform average.
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
                vm.Tables.Add(index.Name);
                if (index.Type == TableTypes.ATTRIBUTE)
                    vm.CalcForm.AttributeTables.Add(index.Name);
                else if (index.Type == TableTypes.UPSTREAM)
                    vm.CalcForm.UpstreamTables.Add(index.Name);
            }

            vm.ConvertedTables = new List<ConvertedTablesVM>();
            foreach (string convertedFile in _conversionFileAccessor.GetFiles())
            {
                vm.ConvertedTables.Add(new ConvertedTablesVM()
                {
                    TableID = 0,
                    Table = Path.GetFileNameWithoutExtension(convertedFile),
                    Issuer = "",
                    Time = "",
                    Type = ""
                });
            }

            return vm;
        }

        #endregion Helpers
    }
}
