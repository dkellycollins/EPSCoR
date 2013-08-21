using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using EPSCoR.Web.App;
using EPSCoR.Web.App.Hubs;
using EPSCoR.Web.Database;
using EPSCoR.Web.Database.Context;
using EPSCoR.Web.Database.Models;
using EPSCoR.Web.Database.Services;

namespace EPSCoR.Web.App
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private DbWatcher _dbWatcher;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RouteTable.Routes.MapHubs();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            DirectoryManager.SetRootDirectory(Server.MapPath("~/App_Data"));

            ModelDbContext.ModelCreated += ModelCreated;
            ModelDbContext.ModelRemoved += ModelRemoved;
            ModelDbContext.ModelUpdated += ModelUpdated;

            _dbWatcher = new DbWatcher();
            _dbWatcher.ModelCreated += ModelCreated;
            _dbWatcher.ModelRemoved += ModelRemoved;
            _dbWatcher.ModelUpdated += ModelUpdated;
            _dbWatcher.Start();
        }

        void ModelUpdated(Model model)
        {
            if (model is TableIndex)
            {
                TableHub.NotifyTableUpdated((TableIndex)model);
            }
        }

        void ModelRemoved(Model model)
        {
            if (model is TableIndex)
            {
                TableHub.NotifyTableRemoved((TableIndex)model);
            }
        }

        void ModelCreated(Model model)
        {
            if (model is TableIndex)
            {
                TableHub.NotifyNewTable((TableIndex)model);
            }
        }
    }
}