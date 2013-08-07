using System.Web.Mvc;

namespace EPSCoR.Filters
{
    public enum ResponseFormats
    {
        None = 0x0,
        Json = 0x1,
        Xml = 0x2,
        Csv = 0x4,
        All =  Json | Xml | Csv
    }

    public class MultipleResponseFormatsAttribute : ActionFilterAttribute
    {
        private ResponseFormats _supportedFormates;

        public MultipleResponseFormatsAttribute()
            : this(ResponseFormats.All)
        { }

        public MultipleResponseFormatsAttribute(ResponseFormats supportedFormats)
        {
            _supportedFormates = supportedFormats;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var viewResult = filterContext.Result as ViewResult;

            if (viewResult == null)
                return;

            if (request.IsAjaxRequest())
            {
                filterContext.Result = new PartialViewResult
                {
                    TempData = viewResult.TempData,
                    ViewData = viewResult.ViewData,
                    ViewName = viewResult.ViewName
                };
            }
            else if (request["format"] == "json")
            {
                filterContext.Result = new JsonResult
                {
                    Data = viewResult.Model
                };
            }
        }
    }
}