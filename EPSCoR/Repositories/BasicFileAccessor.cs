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

        /// <summary>
        /// Temp directory is where we store parts of files while we are uploading.
        /// </summary>
        public const string TEMP_DIRECTORY = "~/App_Data/Temp";

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

        public bool SaveFiles(params FileStreamWrapper[] files)
        {
            waitForLock();

            bool result = true;
            foreach (FileStreamWrapper file in files)
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

        public MemoryStream OpenFile(string fileName)
        {
            string path = Path.Combine(_serverPath, fileName);
            FileStream fileStream;
            MemoryStream returnStream = new MemoryStream();
            
            try
            {
                fileStream = File.Open(path, FileMode.Open, FileAccess.Read);
                fileStream.CopyTo(returnStream);
                returnStream.Seek(0, SeekOrigin.Begin);
                fileStream.Close();
                return returnStream;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return null;
            }
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

        private bool saveFile(FileStreamWrapper file)
        {
            bool result = true;
            var fileName = Path.GetFileName(file.FileName);
            var path = Path.Combine(_serverPath, fileName);

            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                    file.InputStream.CopyTo(fs);
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