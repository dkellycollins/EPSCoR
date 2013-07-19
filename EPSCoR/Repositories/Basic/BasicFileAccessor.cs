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
        private string _user;

        public BasicFileAccessor(string userName)
        {   
            _user = userName;
        }

        #region IFileAccessor Members

        public FileDirectory CurrentDirectory { get; set; }

        public bool SaveFiles(params FileStreamWrapper[] files)
        {
            bool result = true;
            foreach (FileStreamWrapper file in files)
            {
                if (!saveFile(file, FileMode.Create))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        public FileStream OpenFile(string fileName)
        {
            string path = Path.Combine(getUserDirectory(), fileName);
            
            try
            {
                return File.Open(path, FileMode.OpenOrCreate, FileAccess.Read);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return null;
            }
        }

        public void CloseFile(FileStream fileStream)
        {
            fileStream.Close();
            //releaseLock();
        }

        public IEnumerable<string> GetFiles()
        {
            return Directory.GetFiles(getUserDirectory());
        }

        public void DeleteFiles(params string[] fileNames)
        {
            //waitForLock();

            foreach (string fileName in fileNames)
                deleteFile(fileName);

            //releaseLock();
        }

        public bool FileExist(string fileName)
        {
            string path = Path.Combine(getUserDirectory(), fileName);
            return File.Exists(path);
        }

        public FileInfo GetFileInfo(string fileName)
        {
            string path = Path.Combine(getUserDirectory(), fileName);
            return new FileInfo(path);
        }

        #endregion IFileAccessor Memebers

        #region Private Members

        private bool saveFile(FileStreamWrapper file, FileMode fileMode)
        {
            bool result = true;
            var fileName = Path.GetFileName(file.FileName);
            var path = Path.Combine(getUserDirectory(), fileName);

            try
            {
                //If the file does not exist create a new empty file.
                FileStream fileStream = File.Open(path, FileMode.OpenOrCreate);
                
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
            string userDirectory = getUserDirectory();
            if (Path.HasExtension(fileName))
            {
                string path = Path.Combine(userDirectory, fileName);
                if (File.Exists(path))
                    File.Delete(path);
            }
            else
            {
                string[] fileNames = Directory.GetFiles(userDirectory, fileName + ".*");
                foreach (string fn in fileNames)
                {
                    File.Delete(fn);
                }
            }
        }

        private string getUserDirectory()
        {
            string userDirectory;
            switch (CurrentDirectory)
            {
                case FileDirectory.Archive:
                    userDirectory = Path.Combine(DirectoryManager.ArchiveDir, _user);
                    break;
                case FileDirectory.Conversion:
                    userDirectory = Path.Combine(DirectoryManager.ConversionDir, _user);
                    break;
                case FileDirectory.Invalid:
                    userDirectory = Path.Combine(DirectoryManager.InvalidDir, _user);
                    break;
                case FileDirectory.Temp:
                    userDirectory = Path.Combine(DirectoryManager.TempDir, _user);
                    break;
                case FileDirectory.Upload:
                    userDirectory = Path.Combine(DirectoryManager.UploadDir, _user);
                    break;
                default:
                    throw new Exception("Unknown Directory");
            }

            if (!Directory.Exists(userDirectory))
                Directory.CreateDirectory(userDirectory);

            return userDirectory;
        }

        #endregion Private Members
    }
}