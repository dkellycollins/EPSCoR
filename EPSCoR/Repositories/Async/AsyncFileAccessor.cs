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
    public class AsyncFileAccessor : IAsyncFileAccessor
    {
        private string _user;

        public AsyncFileAccessor(string userName)
        {
            _user = userName;
        }

        public FileDirectory CurrentDirectory { get; set; }

        #region IAsyncFileAccessor Members

        public async Task<bool> SaveFilesAsync(params FileStreamWrapper[] files)
        {
            IEnumerable<Task<bool>> tasks = from file in files
                                            select saveFileTaskAsync(file, FileMode.Create);

            bool[] results = await Task.WhenAll(tasks.ToArray());

            return results.All(r => r);
        }

        public async Task<FileStream> OpenFileAsync(string fileName)
        {
            string path = Path.Combine(getUserDirectory(), fileName);

            return await Task.Run(() => 
            {
                try
                {
                    return File.Open(path, FileMode.OpenOrCreate, FileAccess.Read);
                }
                catch(Exception e)
                {
                    return null;
                }
            });
        }

        public async Task<IEnumerable<string>> GetFilesAsync()
        {
            return await Task.Run(() => Directory.GetFiles(getUserDirectory()));
        }

        public async Task DeleteFilesAsync(params string[] fileNames)
        {
            IEnumerable<Task> tasks = from fileName in fileNames
                                      select deleteFileAsync(fileName);

            await Task.WhenAll(tasks);
        }

        public async Task<bool> FileExistAsync(string fileName)
        {
            return await Task.Run(() =>
            {
                string path = Path.Combine(getUserDirectory(), fileName);
                return File.Exists(path);
            });
        }

        public async Task<FileInfo> GetFileInfoAsync(string fileName)
        {
            return await Task.Run(() =>
            {
                string path = Path.Combine(getUserDirectory(), fileName);
                return new FileInfo(path);
            });
        }

        #endregion IAsyncFileAccessor Members

        #region Private Members

        private async Task<bool> saveFileTaskAsync(FileStreamWrapper file, FileMode fileMode)
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
                await file.InputStream.CopyToAsync(fileStream);
                fileStream.Flush();
                fileStream.Close();
            }
            catch (Exception e)
            {
                result = false;
            }
            return result;
        }

        private async Task deleteFileAsync(string fileName)
        {
            string userDirectory = getUserDirectory();
            if (Path.HasExtension(fileName))
            {
                await Task.Run(() =>
                {
                    string path = Path.Combine(userDirectory, fileName);
                    if (File.Exists(path))
                        File.Delete(path);
                });
            }
            else
            {
                await Task.Run(() =>
                {
                    string[] fileNames = Directory.GetFiles(userDirectory, fileName + ".*");
                    foreach (string fn in fileNames)
                    {
                        File.Delete(fn);
                    }
                });
            }
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
    }
}