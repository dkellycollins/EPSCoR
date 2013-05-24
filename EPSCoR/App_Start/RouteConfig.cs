using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EPSCoR.Controllers;
using NavigationRoutes;

namespace EPSCoR
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapNavigationRoute<HomeController>("Home", c => c.Index());
            //routes.MapNavigationRoute<AboutController>("About", c => c.Index());
            routes.MapNavigationRoute<AboutController>("How To", c => c.Index())
                .AddChildRoute<AboutController>("Export Access Table", c => c.HowToExport())
                .AddChildRoute<AboutController>("Upload Data Table", c => c.HowToUpload());
            routes.MapNavigationRoute<TablesController>("Load Tables", c => c.Upload());
            routes.MapNavigationRoute<TablesController>("Data", c => c.Index());
            //routes.MapNavigationRoute<WatershedController>("Watershed", c => c.Index());
        }
    }
}