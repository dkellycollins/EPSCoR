using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EPSCoR.Controllers
{
    public class FileHelpers
    {
        private const string DATA_DIRECTORY = "~/App_Data/Uploads";

        private static HttpServerUtility Server
        {
            get { return System.Web.HttpContext.Current.Server; }
        }

        public static string SaveFile(string userName, HttpPostedFileBase file)
        {
            var fileName = Path.GetFileName(file.FileName);
            var directory = Server.MapPath(DATA_DIRECTORY);
            var path = Path.Combine(directory, userName, fileName);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            file.SaveAs(path);

            return path;
        }
    }
}