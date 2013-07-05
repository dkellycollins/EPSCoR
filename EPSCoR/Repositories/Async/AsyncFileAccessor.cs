using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using EPSCoR.Database.Services;

namespace EPSCoR.Repositories.Async
{
    public class AsyncFileAccessor : IAsyncFileAccessor, IFileAccessor
    {
        private string _user;

        public AsyncFileAccessor(string userName)
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
                //if (!saveFile(file, FileMode.Create))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        public async Task<bool> SaveFilesAsync(params FileStreamWrapper[] files)
        {
            IEnumerable<Task<bool>> tasks = from file in files 
                                   select saveFileTaskAsync(file, FileMode.Create);

            Task.WaitAll(tasks.ToArray());

            return tasks.Any(t => !t.Result);
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

        public IEnumerable<string> GetFiles()
        {
            return Directory.GetFiles(getUserDirectory());
        }

        public void DeleteFiles(params string[] fileNames)
        {
            foreach (string fileName in fileNames)
                deleteFile(fileName);
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

        private async Task<bool> saveFileTaskAsync(FileStreamWrapper file, FileMode fileMode)
        {
            bool result = true;
            var fileName = Path.GetFileName(file.FileName);
            var path = Path.Combine(getUserDirectory(), fileName);

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
            string path = Path.Combine(getUserDirectory(), fileName);
            if (File.Exists(path))
                File.Delete(path);
        }

        private string getUserDirectory()
        {
            string dir = string.Empty;
            switch (CurrentDirectory)
            {
                case FileDirectory.Archive:
                    dir = Path.Combine(DirectoryManager.ArchiveDir, _user);
                    break;
                case FileDirectory.Conversion:
                    dir = Path.Combine(DirectoryManager.ConversionDir, _user);
                    break;
                case FileDirectory.Invalid:
                    dir = Path.Combine(DirectoryManager.InvalidDir, _user);
                    break;
                case FileDirectory.Temp:
                    dir = Path.Combine(DirectoryManager.TempDir, _user);
                    break;
                case FileDirectory.Upload:
                    dir = Path.Combine(DirectoryManager.UploadDir, _user);
                    break;
                default:
                    throw new Exception("Unknown Directory");
            }

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }

        #endregion Private Members


        public void DeleteFilesAsync(params string[] fileNames)
        {
            throw new NotImplementedException();
        }

        public Task<FileStream> OpenFileAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetFilesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<FileInfo> GetFileInfoAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FileExistAsync(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}