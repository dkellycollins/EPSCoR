using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPSCoR.ViewModels
{
    /// <summary>
    /// Wraps up the parameter for a calc request.
    /// </summary>
    [ModelBinder(typeof(CalcRequestBinder))]
    public class CalcRequest
    {
        /// <summary>
        /// The attribute table to use to generate the new table.
        /// </summary>
        public string AttributeTable { get; set; }

        /// <summary>
        /// The upstream table to use to generate the new table.
        /// </summary>
        public string UpstreamTable { get; set; }

        /// <summary>
        /// The type of calculation to perform on the new table.
        /// </summary>
        public string CalcType { get; set; }

        public class CalcRequestBinder : IModelBinder
        {
            /// <summary>
            /// Pulls the calcrequest fields from the http request.
            /// </summary>
            /// <param name="controllerContext"></param>
            /// <param name="bindingContext"></param>
            /// <returns></returns>
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