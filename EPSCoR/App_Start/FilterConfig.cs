using System.Web.Mvc;
using EPSCoR.Filters;

namespace EPSCoR
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new RequireRemoteHttpsAttribute());
        }
    }
}