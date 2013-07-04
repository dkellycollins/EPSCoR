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

        public IFileAccessor.FileDirectory CurrentDirectory { get; set; }

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
            string path = Path.Combine(getUserDirectory(), fileName);
            if (File.Exists(path))
                File.Delete(path);
        }

        private string getUserDirectory()
        {
            switch (CurrentDirectory)
            {
                case IFileAccessor.FileDirectory.Archive:
                    return Path.Combine(DirectoryManager.ArchiveDir, _user);
                case IFileAccessor.FileDirectory.Conversion:
                    return Path.Combine(DirectoryManager.ConversionDir, _user);
                case IFileAccessor.FileDirectory.Invalid:
                    return Path.Combine(DirectoryManager.InvalidDir, _user);
                case IFileAccessor.FileDirectory.Temp:
                    return Path.Combine(DirectoryManager.TempDir, _user);
                case IFileAccessor.FileDirectory.Upload:
                    return Path.Combine(DirectoryManager.UploadDir, _user);
                default:
                    throw new Exception("Unknown Directory");
            }
        }

        #endregion Private Members
    }
}