using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace EPSCoR.Results
{
    /// <summary>
    /// This wraps up data to be serailized into the Json object returned.
    /// </summary>
    public class FileUploadResult : NewtonsoftJsonResult
    {
        public string Name { get; set; }
        public string Error { get; set; }
        public int UploadedBytes { get; set; }

        public FileUploadResult() { }

        public FileUploadResult(string fileName, string error = null)
        {
            Name = fileName;
            Error = error;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            this.Data = new
            {
                Name = Name,
                Error = Error,
                UploadedBytes = UploadedBytes
            };

            base.ExecuteResult(context);
        }
    }
}