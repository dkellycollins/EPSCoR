using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.Results
{
    public class CheckFileResult : NewtonsoftJsonResult
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The size of the file that has already been uploaded.
        /// </summary>
        public int UploadedBytes { get; set; }

        /// <summary>
        /// True if all or part of the file has been uploaded.
        /// </summary>
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