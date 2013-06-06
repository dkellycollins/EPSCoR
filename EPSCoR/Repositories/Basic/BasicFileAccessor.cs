using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using EPSCoR.Database.Services;

namespace EPSCoR.Repositories.Basic
{
    public class BasicFileAccessor : IFileAccessor
    {
        public static BasicFileAccessor GetUploadAccessor(string userName)
        {
            return new BasicFileAccessor(DirectoryManager.UploadDir, userName);
        }

        public static BasicFileAccessor GetConversionsAccessor(string userName)
        {
            return new BasicFileAccessor(DirectoryManager.ConversionDir, userName);
        }

        public static BasicFileAccessor GetTempAccessor(string userName)
        {
            return new BasicFileAccessor(DirectoryManager.TempDir, userName);
        }

        private string _userDirectory;
        private string _lockFile;

        private BasicFileAccessor(string directory, string userName)
        {   
            _userDirectory = Path.Combine(directory, userName);
            if (!Directory.Exists(_userDirectory))
                Directory.CreateDirectory(_userDirectory);
            
            _lockFile = Path.Combine(_userDirectory, "lock");
        }

        #region IFileAccessor Members

        public bool SaveFiles(params FileStreamWrapper[] files)
        {
            waitForLock();

            bool result = true;
            foreach (FileStreamWrapper file in files)
            {
                if (!saveFile(file, FileMode.Create))
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
            waitForLock();

            string path = Path.Combine(_userDirectory, fileName);
            
            try
            {
                return File.Open(path, FileMode.OpenOrCreate, FileAccess.Read);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                releaseLock();
                return null;
            }
        }

        public void CloseFile(FileStream fileStream)
        {
            fileStream.Close();
            releaseLock();
        }

        public IEnumerable<string> GetFiles()
        {
            return Directory.GetFiles(_userDirectory);
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
            string path = Path.Combine(_userDirectory, fileName);
            return File.Exists(path);
        }

        public FileInfo GetFileInfo(string fileName)
        {
            string path = Path.Combine(_userDirectory, fileName);
            return new FileInfo(path);
        }

        #endregion IFileAccessor Memebers

        #region Private Members

        private bool saveFile(FileStreamWrapper file, FileMode fileMode)
        {
            bool result = true;
            var fileName = Path.GetFileName(file.FileName);
            var path = Path.Combine(_userDirectory, fileName);

            try
            {
                //If the file does not exist create a new empty file.
                if (!File.Exists(path))
                    File.WriteAllBytes(path, new byte[file.FileSize]);
                FileStream fileStream = File.Open(path, FileMode.Open);
                
                //Seek to the staring position of the chunk and copy the stream.
                fileStream.Seek(file.SeekPos, SeekOrigin.Begin);
                file.InputStream.CopyTo(fileStream);
                fileStream.Flush();
                fileStream.Close();
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
            string path = Path.Combine(_userDirectory, fileName);
            if (File.Exists(path))
                File.Delete(path);
        }

        private void waitForLock()
        {
            while (Directory.GetFiles(_userDirectory).Contains(_lockFile)) ;
            File.Create(_lockFile).Close();
        }

        private void releaseLock()
        {
            File.Delete(_lockFile);
        }

        #endregion Private Members
    }
}