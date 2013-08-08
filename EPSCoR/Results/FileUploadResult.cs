using System.Web.Mvc;

namespace EPSCoR.Results
{
    /// <summary>
    /// This wraps up data to be serailized into the Json object returned.
    /// </summary>
    public class FileUploadResult : NewtonsoftJsonResult
    {
        /// <summary>
        /// Name of the file uploaded.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The error message if there was an error other wise false.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// How many bytes have been uploaded thus far.
        /// </summary>
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