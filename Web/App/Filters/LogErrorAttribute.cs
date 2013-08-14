using System;
using System.Web.Mvc;

namespace EPSCoR.Web.App.Filters
{
    public class LogErrorAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            Console.Out.WriteLine("An internal exception occured: " + filterContext.Exception.Message + "\n" + filterContext.Exception.StackTrace);
        }
    }
}