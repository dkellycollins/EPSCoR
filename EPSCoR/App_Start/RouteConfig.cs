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

            /*
            routes.MapRoute(
                name: "ApiRoute",
                url: "api/{controller}/{action}/",
                namespaces: new string[] { "EPSCoR.Controllers.API" }
            );
             */

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "EPSCoR.Controllers" }
            );
        }
    }
}