using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace EPSCoR.Results
{
    public class JsonResult : System.Web.Mvc.JsonResult
    {
        public JsonResult()
            : base()
        {
            this.ContentType = "application/json";
        }

        public JsonResult(object data)
            : base()
        {
            this.ContentType = "application/json";
            this.Data = data;
        }

        public override void ExecuteResult(System.Web.Mvc.ControllerContext context)
        {
            var response = context.HttpContext.Response;

            response.ContentType = this.ContentType;

            string json = JsonConvert.SerializeObject(this.Data);
            response.Write(json);
        } 
    }
}