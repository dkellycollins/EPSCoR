using System.Web.Mvc;
using Newtonsoft.Json;

namespace EPSCoR.Web.App.Results
{
    /// <summary>
    /// Serilizes data using Newtonsoft.Json.JsonConvert.
    /// </summary>
    public class NewtonsoftJsonResult : ActionResult
    {
        /// <summary>
        /// The data to be serialized.
        /// </summary>
        protected object Data { get; set; }

        public NewtonsoftJsonResult()
            : base()
        { }

        public NewtonsoftJsonResult(object data)
            : base()
        {
            this.Data = data;
        }

        /// <summary>
        /// Serailizes the data and writes it to the response.
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(System.Web.Mvc.ControllerContext context)
        {
            var response = context.HttpContext.Response;

            response.ContentType = "application/json";

            string json = JsonConvert.SerializeObject(this.Data);
            response.Write(json);
        } 
    }
}