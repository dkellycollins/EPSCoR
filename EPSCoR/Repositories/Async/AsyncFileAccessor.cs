using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EPSCoR.Repositories.Async
{
    /*public class AsyncFileAccessor : IFileAccessor
    {
        private string _userDirectory;
        private string _lockFile;

        public AsyncFileAccessor(string directory, string userName)
        {
            _userDirectory = Path.Combine(directory, userName);
            if (!Directory.Exists(_userDirectory))
                Directory.CreateDirectory(_userDirectory);

            _lockFile = Path.Combine(_userDirectory, "lock");
        }

        #region IFileAccessor Members

        public bool SaveFiles(params FileStreamWrapper[] files)
        {
            //waitForLock();

            bool result = true;
            foreach (FileStreamWrapper file in files)
            {
                if (!saveFile(file, FileMode.Create))
                {
                    result = false;
                    break;
                }
            }

            //releaseLock();

            return result;
        }

        public async Task<bool> SaveFilesTaskAsync(params FileStreamWrapper[] files)
        {
            Task<bool[]>[] tasks = from file in files 
                                   select saveFileTaskAsync(file, FileMode.Create);

            Task.WaitAll(tasks);
        }

        public FileStream OpenFile(string fileName)
        {
            //waitForLock();

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
            //releaseLock();
        }

        public IEnumerable<string> GetFiles()
        {
            return Directory.GetFiles(_userDirectory);
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

        private async Task<bool> saveFileTaskAsync(FileStreamWrapper file, FileMode fileMode)
        {
            bool result = true;
            var fileName = Path.GetFileName(file.FileName);
            var path = Path.Combine(_userDirectory, fileName);

            return await Task.Run(() =>
            {
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
            });
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
    }*/
}