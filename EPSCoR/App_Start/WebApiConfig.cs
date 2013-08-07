using System.Web.Http;

namespace EPSCoR
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //Might move this to Route config.
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
