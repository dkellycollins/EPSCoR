using System.Web.Mvc;
using EPSCoR.Web.App.Results;

namespace EPSCoR.Web.App.Filters
{
    /// <summary>
    /// The avalible response formats.
    /// </summary>
    public enum ResponseFormat
    {
        None = 0x0,
        Json = 0x1,
        Ajax = 0x2,
        All =  Json | Ajax
    }

    /// <summary>
    /// Allows any controller to return the result in different formats depending on how the request was made.
    /// </summary>
    public class MultipleResponseFormatsAttribute : ActionFilterAttribute
    {
        private ResponseFormat _supportedFormates;

        /// <summary>
        /// Allows all response types.
        /// </summary>
        public MultipleResponseFormatsAttribute()
            : this(ResponseFormat.All)
        { }

        /// <summary>
        /// Allows only the specified response formats. (Multiple formats should be ored together)
        /// </summary>
        /// <param name="supportedFormats"></param>
        public MultipleResponseFormatsAttribute(ResponseFormat supportedFormats)
        {
            _supportedFormates = supportedFormats;
        }

        /// <summary>
        /// Check to see if we need to change the format of the response and do so if we do.
        /// </summary>
        /// <param name="filterContext"></param>
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