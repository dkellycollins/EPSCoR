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
        private IRepositoryFactory _repoFactory;

        public TablesController()
        {
            _repoFactory = new RepositoryFactory();
        }

        public TablesController(IRepositoryFactory factory)
        {
            _repoFactory = factory;
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
            DataTable table = null;
            using (IAsyncTableRepository repo = _repoFactory.GetAsyncTableRepository(WebSecurity.CurrentUserName))
            {
                table = await repo.ReadAsync(id, lowerLimit, upperLimit);
            }

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
            DataTable data = null; 
            int totalRows = 0; 
            int echo = Int32.Parse(args.Echo);

            using (IAsyncTableRepository repo = _repoFactory.GetAsyncTableRepository(WebSecurity.CurrentUserName))
            {
                data = await repo.ReadAsync(args.TableName, args.DisplayStart, args.DisplayLength);
                totalRows = await repo.CountAsync(args.TableName);
            }

            return new DataTableResult(totalRows, totalRows, echo, data);
        }

        /// <summary>
        /// Gets all table indexes and returns them in Json format.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllDetails()
        {
            string userName = WebSecurity.CurrentUserName;
            using (IModelRepository<TableIndex> repo = _repoFactory.GetModelRepository<TableIndex>())
            {
                IEnumerable<TableIndex> allDetails = repo.Where((index) => index.UploadedByUser == userName);
                return new NewtonsoftJsonResult(allDetails);
            }
        }
    }
}
