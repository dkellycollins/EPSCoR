using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace EPSCoR.Results
{
    public class FileUploadResult : ActionResult
    {
        /// <summary>
        /// This wraps up data to be serailized into the Json object returned.
        /// </summary>
        public class FileStatus
        {
            public string Name { get; set; }
            public string Error { get; set; }
        }

        private JavaScriptSerializer _serializer;
        private FileStatus _status;

        public FileUploadResult(FileStatus status)
        {
            _serializer = new JavaScriptSerializer();
            _status = status;
        }

        public FileUploadResult(string FileName, string error = null)
        {
            _serializer = new JavaScriptSerializer();
            _status = new FileStatus()
            {
                Name = FileName,
                Error = error
            };
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;

            response.AddHeader("Vary", "Accept");
            try
            {
                if (request["HTTP_ACCEPT"].Contains("application/json"))
                    response.ContentType = "application/json";
                else
                    response.ContentType = "text/plain";
            }
            catch
            {
                response.ContentType = "text/plain";
            }

            var jsonObj = _serializer.Serialize(_status);
            response.Write(jsonObj);
        }
    }
}