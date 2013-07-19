using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPSCoR.Filters
{
    public class LogErrorAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            Console.Out.WriteLine("An internal exception occured: " + filterContext.Exception.Message + "\n" + filterContext.Exception.StackTrace);
        }
    }
}