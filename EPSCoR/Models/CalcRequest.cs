using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPSCoR.Models
{
    [ModelBinder(typeof(CalcRequestBinder))]
    public class CalcRequest
    {
        public string AttributeTable { get; set; }
        public string UpstreamTable { get; set; }
        public string CalcType { get; set; }

        public class CalcRequestBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var request = controllerContext.HttpContext.Request;
                var formCollection = request.Form;

                string attTable = formCollection["attTable"];
                string usTable = formCollection["usTable"];
                string calc = formCollection["calc"];

                return new CalcRequest()
                {
                    AttributeTable = attTable,
                    UpstreamTable = usTable,
                    CalcType = calc
                };
            }
        }
    }
}