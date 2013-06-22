using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EPSCoR.Models
{
    [ModelBinder(typeof(ModelBinder))]
    public class FileUpload
    {
        public string FileName { get; set; }
        public Stream InputStream { get; set; }
        public int StartPosition { get; set; }
        public int TotalFileLength { get; set; }

        public class ModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var request = controllerContext.RequestContext.HttpContext.Request;

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