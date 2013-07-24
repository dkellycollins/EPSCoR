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

        public bool SaveFiles(FileDirectory directory, params FileStreamWrapper[] files)
        {
            bool result = true;
            foreach (FileStreamWrapper file in files)
            {
                if (!saveFile(directory, file, FileMode.Create))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        public FileStream OpenFile(FileDirectory directory, string fileName)
        {
            string path = Path.Combine(getUserDirectory(directory), fileName);
            
            try
            {
                return File.Open(path, FileMode.OpenOrCreate, FileAccess.Read);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public IEnumerable<string> GetFiles(FileDirectory directory)
        {
            return Directory.GetFiles(getUserDirectory(directory));
        }

        public void DeleteFiles(FileDirectory directory, params string[] fileNames)
        {
            foreach (string fileName in fileNames)
                deleteFile(directory, fileName);
        }

        public bool FileExist(FileDirectory directory, string fileName)
        {
            string path = Path.Combine(getUserDirectory(directory), fileName);
            return File.Exists(path);
        }

        public FileInfo GetFileInfo(FileDirectory directory, string fileName)
        {
            string path = Path.Combine(getUserDirectory(directory), fileName);
            return new FileInfo(path);
        }

        #endregion IFileAccessor Memebers

        #region Private Members

        private bool saveFile(FileDirectory directory, FileStreamWrapper file, FileMode fileMode)
        {
            bool result = true;
            var fileName = Path.GetFileName(file.FileName);
            var path = Path.Combine(getUserDirectory(directory), fileName);

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
                result = false;
            }
            return result;
        }

        private void deleteFile(FileDirectory directory, string fileName)
        {
            string userDirectory = getUserDirectory(directory);
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

        private string getUserDirectory(FileDirectory directory)
        {
            string userDirectory;
            switch (directory)
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