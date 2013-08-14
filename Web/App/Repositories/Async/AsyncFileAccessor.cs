using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EPSCoR.Web.App.Repositories.Async
{
    public class AsyncFileAccessor : IAsyncFileAccessor
    {
        private string _user;
        private IDirectoryResolver _directoryResolver;

        public AsyncFileAccessor(string userName, IDirectoryResolver directoryResolver)
        {
            _user = userName;
            _directoryResolver = directoryResolver;
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
            string path = Path.Combine(_directoryResolver.GetUserDirectory(directory, _user), fileName);

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
            return await Task.Run(() => Directory.GetFiles(_directoryResolver.GetUserDirectory(directory, _user)));
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
                string path = Path.Combine(_directoryResolver.GetUserDirectory(directory, _user), fileName);
                return File.Exists(path);
            });
        }

        public async Task<FileInfo> GetFileInfoAsync(FileDirectory directory, string fileName)
        {
            return await Task.Run(() =>
            {
                string path = Path.Combine(_directoryResolver.GetUserDirectory(directory, _user), fileName);
                return new FileInfo(path);
            });
        }

        public async Task MoveFileAsync(FileDirectory currentDirectory, FileDirectory newDirectory, string fileName)
        {
            await Task.Run(() =>
            {
                string currentFilePath = Path.Combine(_directoryResolver.GetUserDirectory(currentDirectory, _user), fileName);
                string newFilePath = Path.Combine(_directoryResolver.GetUserDirectory(newDirectory, _user), fileName);

                File.Move(currentFilePath, newFilePath);
            });
        }

        public async Task<string> GenerateFileKeyAsync(FileDirectory directory, string fileName)
        {
            /*using (FileStream fileStream = await OpenFileAsync(directory, fileName))
            {
                return await Task.Run(() =>
                {
                    MD5 hasher = MD5.Create();
                    byte[] hash = hasher.ComputeHash(fileStream);
                    return Encoding.Default.GetString(hash);
                });
            }*/
            using (BufferedStream fileStream = new BufferedStream(await OpenFileAsync(directory, fileName)))
            {
                return await Task.Run(() =>
                {
                    MD5 hasher = MD5.Create();
                    byte[] hash = hasher.ComputeHash(fileStream);
                    return Encoding.Default.GetString(hash);
                });
            }
            /*return Task.Run(() =>
            {
                Process p = new Process();
                p.StartInfo.FileName = "md5sum.exe";
                p.StartInfo.Arguments = Path.Combine(_directoryResolver.GetUserDirectory(directory, _user), fileName);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
                p.WaitForExit();
                string output = p.StandardOutput.ReadToEnd();
                return output.Split(' ')[0].Substring(1).ToUpper();
             });
             */
        }

        #endregion IAsyncFileAccessor Members

        #region Private Members

        private async Task<bool> saveFileTaskAsync(FileDirectory directory, FileStreamWrapper file, FileMode fileMode)
        {
            bool result = true;
            var fileName = Path.GetFileName(file.FileName);
            var path = Path.Combine(_directoryResolver.GetUserDirectory(directory, _user), fileName);

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
            string userDirectory = _directoryResolver.GetUserDirectory(directory, _user);
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

        #endregion Private Members
    }
}