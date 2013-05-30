﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace EPSCoR.Controllers
{
    /// These classes were taken from https://github.com/Widen/fine-uploader-server/blob/master/ASP.NET%20MVC%20C%23

    [ModelBinder(typeof(ModelBinder))]
    public class FineUpload
    {
        private const String FILENAME_PARAM = "qqfile";
        private const String PART_INDEX_PARAM = "qqpartindex";
        private const String FILE_SIZE_PARAM = "qqtotalfilesize";
        private const String TOTAL_PARTS_PARAM = "qqtotalparts";
        private const String UUID_PARAM = "qquuid";
        private const String PART_FILENAME_PARAM = "qqfilename";
        private const String BLOB_NAME_PARAM = "qqblobname";
        private const String GENERATE_ERROR_PARAM = "generateError";

        public string FileName { get; set; }
        public int PartIndex { get; set; }
        public int TotalParts { get; set; }
        public Stream InputStream { get; set; }

        public void SaveAs(string destination, bool overwrite = false, bool autoCreateDirectory = true)
        {
            if (autoCreateDirectory)
            {
                var directory = new FileInfo(destination).Directory;
                if (directory != null) directory.Create();
            }

            using (var file = new FileStream(destination, overwrite ? FileMode.Create : FileMode.CreateNew))
                InputStream.CopyTo(file);
        }

        public class ModelBinder : IModelBinder
        {
            public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var request = controllerContext.RequestContext.HttpContext.Request;
                var formUpload = request.Files.Count > 0;

                // find filename
                var fileName = request.Params[PART_FILENAME_PARAM];
                var part = request.Params[PART_INDEX_PARAM];
                var totalparts = request.Params[TOTAL_PARTS_PARAM];

                var upload = new FineUpload
                {
                    FileName = fileName,
                    InputStream = formUpload ? request.Files[0].InputStream : request.InputStream,
                    PartIndex = Convert.ToInt32(part),
                    TotalParts = Convert.ToInt32(totalparts)
                };

                return upload;
            }
        }
    }

    /// <remarks>
    /// Docs at https://github.com/Widen/fine-uploader/blob/master/server/readme.md
    /// </remarks>
    public class FineUploaderResult : ActionResult
    {
        public const string ResponseContentType = "text/plain";

        private readonly bool _success;
        private readonly string _error;
        private readonly bool? _preventRetry;
        private readonly JObject _otherData;

        public FineUploaderResult(bool success, object otherData = null, string error = null, bool? preventRetry = null)
        {
            _success = success;
            _error = error;
            _preventRetry = preventRetry;

            if (otherData != null)
                _otherData = JObject.FromObject(otherData);
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = ResponseContentType;

            response.Write(BuildResponse());
        }

        public string BuildResponse()
        {
            var response = _otherData ?? new JObject();
            response["success"] = _success;

            if (!string.IsNullOrWhiteSpace(_error))
                response["error"] = _error;

            if (_preventRetry.HasValue)
                response["preventRetry"] = _preventRetry.Value;

            return response.ToString();
        }
    }
}