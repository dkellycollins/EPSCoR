using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        #region IAsyncFileAccessor Members

        public async Task<bool> SaveFilesAsync(FileDirectory directory, params FileStreamWrapper[] files)
        {
            IEnumerable<Task<bool>> tasks = from file in files
                                            select saveFileTaskAsync(directory, file, FileMode.Create);

            bool[] results = await Task.WhenAll(tasks.ToArray());

            return results.All(r => r);
        }

        public async Task<FileStream> OpenFileAsync(FileDirectory directory, string fileName)
        {
            string path = Path.Combine(getUserDirectory(directory), fileName);

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

        public async Task<IEnumerable<string>> GetFilesAsync(FileDirectory directory)
        {
            return await Task.Run(() => Directory.GetFiles(getUserDirectory(directory)));
        }

        public async Task DeleteFilesAsync(FileDirectory directory, params string[] fileNames)
        {
            IEnumerable<Task> tasks = from fileName in fileNames
                                      select deleteFileAsync(directory, fileName);

            await Task.WhenAll(tasks);
        }

        public async Task<bool> FileExistAsync(FileDirectory directory, string fileName)
        {
            return await Task.Run(() =>
            {
                string path = Path.Combine(getUserDirectory(directory), fileName);
                return File.Exists(path);
            });
        }

        public async Task<FileInfo> GetFileInfoAsync(FileDirectory directory, string fileName)
        {
            return await Task.Run(() =>
            {
                string path = Path.Combine(getUserDirectory(directory), fileName);
                return new FileInfo(path);
            });
        }

        public async Task MoveFileAsync(FileDirectory currentDirectory, FileDirectory newDirectory, string fileName)
        {
            await Task.Run(() =>
            {
                string currentFilePath = Path.Combine(getUserDirectory(currentDirectory), fileName);
                string newFilePath = Path.Combine(getUserDirectory(newDirectory), fileName);

                File.Move(currentFilePath, newFilePath);
            });
        }

        #endregion IAsyncFileAccessor Members

        #region Private Members

        private async Task<bool> saveFileTaskAsync(FileDirectory directory, FileStreamWrapper file, FileMode fileMode)
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

        private async Task deleteFileAsync(FileDirectory directory, string fileName)
        {
            string userDirectory = getUserDirectory(directory);
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

        private string getUserDirectory(FileDirectory directory)
        {
            string dir = string.Empty;
            switch (directory)
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