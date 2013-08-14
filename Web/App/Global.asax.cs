using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using EPSCoR.Web.App;
using EPSCoR.Web.App.Hubs;
using EPSCoR.Web.Database.Context;
using EPSCoR.Web.Database.Models;

namespace EPSCoR
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
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

            ModelDbContext.ModelCreated += DefaultContext_ModelCreated;
            ModelDbContext.ModelRemoved += DefaultContext_ModelRemoved;
            ModelDbContext.ModelUpdated += DefaultContext_ModelUpdated;
        }

        void DefaultContext_ModelUpdated(IModel model)
        {
            if (model is TableIndex)
            {
                TableHub.NotifyTableUpdated((TableIndex)model);
            }
        }

        void DefaultContext_ModelRemoved(IModel model)
        {
            if (model is TableIndex)
            {
                TableHub.NotifyTableRemoved((TableIndex)model);
            }
        }

        void DefaultContext_ModelCreated(IModel model)
        {
            if (model is TableIndex)
            {
                TableHub.NotifyNewTable((TableIndex)model);
            }
        }
    }
}