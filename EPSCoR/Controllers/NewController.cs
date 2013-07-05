using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Factory;
using WebMatrix.WebData;

namespace EPSCoR.Controllers
{
    [Authorize]
    public class NewController : Controller
    {
        //
        // GET: /New/

        public ActionResult Index()
        {
            using (IModelRepository<TableIndex> repo = RepositoryFactory.GetModelRepository<TableIndex>())
            {
                return View(
                    "~/Views/2.0/Index.cshtml",
                    "~/Views/2.0/Layout2.0.cshtml",
                    repo.GetAll()
                        .Where(index => index.UploadedByUser == WebSecurity.CurrentUserName)
                        .ToList()
                );
            }
        }

    }
}
