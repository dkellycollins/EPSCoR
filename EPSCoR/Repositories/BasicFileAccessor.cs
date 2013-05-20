using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace EPSCoR.Repositories
{
    public class BasicFileAccessor : IFileAccessor
    {
        /// <summary>
        /// Upload directory is where we store the users uploads no matter the format.
        /// </summary>
        public const string UPLOAD_DIRECTORY = "~/App_Data/Uploads";

        /// <summary>
        /// Convertion directory is where we store the files that we create the tables with.
        /// </summary>
        public const string CONVERTION_DIRECTORY = "~/App_Data/Convertions";

        /// <summary>
        /// Archive directory is where we store the uploaded files after they have been converted.
        /// </summary>
        public const string ARCHIVE_DIRECTORY = "~/App_Data/Archive";

        //Just a way to make access the server context easier.
        private static HttpServerUtility Server
        {
            get { return System.Web.HttpContext.Current.Server; }
        }

        private string _serverPath;

        public BasicFileAccessor(string directory, string userName)
        {
            string dataDirectory = Server.MapPath(directory);
            if (!Directory.Exists(dataDirectory))
                Directory.CreateDirectory(dataDirectory);
            
            string userDirectory = Path.Combine(dataDirectory, userName);
            if (!Directory.Exists(userDirectory))
                Directory.CreateDirectory(userDirectory);
            
            _serverPath = userDirectory;
        }

        public bool SaveFile(HttpPostedFileBase file)
        {
            var fileName = Path.GetFileName(file.FileName);
            var path = Path.Combine(_serverPath, fileName);

            try
            {
                file.SaveAs(path);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        public FileStream OpenFile(string fileName)
        {
            string path = Path.Combine(_serverPath, fileName);

            FileStream fileStream;
            try
            {
                if (!File.Exists(path))
                    fileStream = File.Create(path);
                fileStream = File.Open(path, FileMode.Open, FileAccess.Read);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return null;
            }
            return fileStream;
        }

        public IEnumerable<string> GetFiles()
        {
            return Directory.GetFiles(_serverPath);
        }

        public void DeleteFile(string fileName)
        {
            string path = Path.Combine(_serverPath, fileName);
            if (File.Exists(path))
                File.Delete(path);
        }

        public bool FileExist(string fileName)
        {
            string path = Path.Combine(_serverPath, fileName);
            return File.Exists(path);
        }
    }
}