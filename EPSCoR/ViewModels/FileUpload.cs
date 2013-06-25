using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPSCoR.ViewModels
{
    [ModelBinder(typeof(ModelBinder))]
    public class FileUpload
    {
        /// <summary>
        /// Name of the file that is being uploaded.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The stream for the file.
        /// </summary>
        public Stream InputStream { get; set; }

        /// <summary>
        /// Where the stream starts in the complete file.
        /// </summary>
        public int StartPosition { get; set; }

        /// <summary>
        /// The length of the complete file.
        /// </summary>
        public int TotalFileLength { get; set; }

        public class ModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var request = controllerContext.RequestContext.HttpContext.Request;

                //TODO validation

                string fileName = request.Files[0].FileName;
                Stream inputStream = request.Files[0].InputStream;
                int startPos;
                int totalFileLength;
                if (request.Headers["Content-Range"] != null)
                {
                    string[] fileInfo = request.Headers["Content-Range"].Split('/', '-');
                    startPos = Int32.Parse(fileInfo[0].Remove(0, 5));
                    totalFileLength = Int32.Parse(fileInfo[2]);
                }
                else
                {
                    startPos = 0;
                    totalFileLength = request.Files[0].ContentLength;
                }

                return new FileUpload()
                {
                    FileName = fileName,
                    InputStream = inputStream,
                    StartPosition = startPos,
                    TotalFileLength = totalFileLength
                };
            }
        }
    }
}