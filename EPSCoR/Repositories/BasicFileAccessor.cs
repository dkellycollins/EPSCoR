using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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
        private string _lockFile;

        public BasicFileAccessor(string directory, string userName)
        {
            string dataDirectory = Server.MapPath(directory);
            if (!Directory.Exists(dataDirectory))
                Directory.CreateDirectory(dataDirectory);
            
            string userDirectory = Path.Combine(dataDirectory, userName);
            if (!Directory.Exists(userDirectory))
                Directory.CreateDirectory(userDirectory);
            
            _serverPath = userDirectory;
            _lockFile = Path.Combine(_serverPath, "lock");
        }

        #region IFileAccessor Members

        public bool SaveFiles(params HttpPostedFileBase[] files)
        {
            waitForLock();

            bool result = true;
            foreach (HttpPostedFileBase file in files)
            {
                if (!saveFile(file))
                {
                    result = false;
                    break;
                }
            }

            releaseLock();

            return result;
        }

        public FileStream OpenFile(string fileName)
        {
            //Might not even need this function. It wont work since I have no way of releasing the lock before returning.
            throw new NotImplementedException();

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

        public void DeleteFiles(params string[] fileNames)
        {
            waitForLock();

            foreach (string fileName in fileNames)
                deleteFile(fileName);

            releaseLock();
        }

        public bool FileExist(string fileName)
        {
            string path = Path.Combine(_serverPath, fileName);
            return File.Exists(path);
        }

        #endregion IFileAccessor Memebers

        #region Private Members

        private bool saveFile(HttpPostedFileBase file)
        {
            bool result = true;
            var fileName = Path.GetFileName(file.FileName);
            var path = Path.Combine(_serverPath, fileName);

            try
            {
                file.SaveAs(path);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                result = false;
            }
            return result;
        }

        private void deleteFile(string fileName)
        {
            string path = Path.Combine(_serverPath, fileName);
            if (File.Exists(path))
                File.Delete(path);
        }

        private void waitForLock()
        {
            while (Directory.GetFiles(_serverPath).Contains(_lockFile)) ;
            File.Create(_lockFile).Close();
        }

        private void releaseLock()
        {
            File.Delete(_lockFile);
        }

        #endregion Private Members
    }
}