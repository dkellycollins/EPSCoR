using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.Web.App.Results
{
    public class CheckFileResult : NewtonsoftJsonResult
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        public string TableName { get; set; }

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

        public CheckFileResult(string tableName, int uploadedBytes, bool fileExists)
        {
            TableName = tableName;
            UploadedBytes = uploadedBytes;
            FileExists = fileExists;
        }

        public override void ExecuteResult(System.Web.Mvc.ControllerContext context)
        {
            Data = new
            {
                tableName = TableName,
                uploadedBytes = UploadedBytes,
                fileExists = FileExists
            };

            base.ExecuteResult(context);
        }
    }
}