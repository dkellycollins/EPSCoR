using System.Web.Mvc;
using EPSCoR.Results;

namespace EPSCoR.Filters
{
    public enum ResponseFormat
    {
        None = 0x0,
        Json = 0x1,
        Ajax = 0x2,
        All =  Json | Ajax
    }

    public class MultipleResponseFormatsAttribute : ActionFilterAttribute
    {
        private ResponseFormat _supportedFormates;

        public MultipleResponseFormatsAttribute()
            : this(ResponseFormat.All)
        { }

        public MultipleResponseFormatsAttribute(ResponseFormat supportedFormats)
        {
            _supportedFormates = supportedFormats;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var viewResult = filterContext.Result as ViewResult;

            if (viewResult == null)
                return;

            //Json
            if (request["format"] == "json" && isFormatAllowed(ResponseFormat.Json))
            {
                filterContext.Result = new NewtonsoftJsonResult(viewResult.Model);
            }
            //Ajax
            else if (request.IsAjaxRequest() && isFormatAllowed(ResponseFormat.Ajax))
            {
                filterContext.Result = new PartialViewResult
                {
                    TempData = viewResult.TempData,
                    ViewData = viewResult.ViewData,
                    ViewName = viewResult.ViewName
                };
            }        
        }

        private bool isFormatAllowed(ResponseFormat format)
        {
            return (_supportedFormates & format) > 0;
        }
    }
}