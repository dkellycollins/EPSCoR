using System.Web;
using System.Web.Mvc;

namespace EPSCoR.Web.App
{
    public class FilterConfig
    {
        /// <summary>
        /// Registers filters that apply to all controllers.
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}