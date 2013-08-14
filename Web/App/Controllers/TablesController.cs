using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using EPSCoR.Web.App.Filters;
using EPSCoR.Web.App.Repositories;
using EPSCoR.Web.App.Repositories.Factory;
using EPSCoR.Web.App.Results;
using EPSCoR.Web.App.ViewModels;
using EPSCoR.Web.Database.Models;
using WebMatrix.WebData;

namespace EPSCoR.Web.App.Controllers
{
    /// <summary>
    /// Contains views that display and work with uploaded tables.
    /// </summary>
    [AddUserWhenAuthorized]
    public class TablesController : Controller
    {
        private IModelRepository<TableIndex> _tableIndexRepo;
        private IAsyncTableRepository _tableRepo;
        private IAsyncDatabaseCalc _dbCalc;

        public TablesController()
        {
            _tableIndexRepo = RepositoryFactory.GetModelRepository<TableIndex>();
            _tableRepo = RepositoryFactory.GetAsyncTableRepository(WebSecurity.CurrentUserName);
            _dbCalc = RepositoryFactory.GetAsyncDatabaseCalc(WebSecurity.CurrentUserName);
        }

        public TablesController(
            IModelRepository<TableIndex> tableIndexRepo,
            IAsyncTableRepository tableRepo,
            IAsyncDatabaseCalc dbCalc)
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
        //[OutputCache(Duration=0, VaryByParam="id, lowerLimit, upperLimit", VaryByCustom="user")]
        [MultipleResponseFormats]
        public async Task<ActionResult> Details(string id, int lowerLimit = 0, int upperLimit = 10)
        {
            DataTable table = await _tableRepo.ReadAsync(id, lowerLimit, upperLimit);
            if (table == null)
            {
                return HttpNotFound();
            }
            return View(table);
        }

        /// <summary>
        /// For use with datatable, return a portion of the table.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task<ActionResult> DataTableDetails(DataTableParams args)
        {
            DataTable data = await _tableRepo.ReadAsync(args.TableName, args.DisplayStart, args.DisplayLength);
            int totalRows = await _tableRepo.CountAsync(args.TableName);
            int echo = Int32.Parse(args.Echo);

            return new DataTableResult(totalRows, totalRows, echo, data);
        }

        /// <summary>
        /// Gets all table indexes and returns them in Json format.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllDetails()
        {
            string userName = WebSecurity.CurrentUserName;
            var tables = _tableIndexRepo.GetAll();
            IEnumerable<TableIndex> allDetails = tables.Where((index) => index.UploadedByUser == userName).ToList();
            return new NewtonsoftJsonResult(allDetails);
        }
    }
}
