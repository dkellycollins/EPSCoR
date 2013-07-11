using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using EPSCoR.Database;
using EPSCoR.Hubs;

namespace EPSCoR
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private FileProcessor _fileProcessor;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RouteTable.Routes.MapHubs();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            _fileProcessor = new FileProcessor();
        }

        void FileProcessor_TableIndexUpdated(Database.Models.TableIndex tableIndex)
        {
            TableHub hub = new TableHub();
            hub.UpdateTable(tableIndex);
        }

        void FileProcessor_TableIndexCreated(Database.Models.TableIndex tableIndex)
        {
            TableHub hub = new TableHub();
            hub.NewTable(tableIndex);
        }

        protected void Application_End()
        {
            _fileProcessor.Dispose();
        }
    }
}