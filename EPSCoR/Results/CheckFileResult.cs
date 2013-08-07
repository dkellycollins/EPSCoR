using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.Results
{
    public class CheckFileResult : NewtonsoftJsonResult
    {
        public string FileName { get; set; }
        public int UploadedBytes { get; set; }
        public bool FileExists { get; set; }

        public CheckFileResult()
        { }

        public CheckFileResult(string fileName, int uploadedBytes, bool fileExists)
        {
            FileName = fileName;
            UploadedBytes = uploadedBytes;
            FileExists = fileExists;
        }

        public override void ExecuteResult(System.Web.Mvc.ControllerContext context)
        {
            Data = new
            {
                fileName = FileName,
                uploadedBytes = UploadedBytes,
                fileExists = FileExists
            };

            base.ExecuteResult(context);
        }
    }
}