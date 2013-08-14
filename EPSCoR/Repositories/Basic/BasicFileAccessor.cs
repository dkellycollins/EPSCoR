using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using EPSCoR.Util;

namespace EPSCoR.Repositories.Basic
{
    public class BasicFileAccessor : IFileAccessor
    {
        private string _user;
        private IDirectoryResolver _directoryResolver;

        public BasicFileAccessor(string userName, IDirectoryResolver directoryResolver)
        {   
            _user = userName;
            _directoryResolver = directoryResolver;
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
            string path = Path.Combine(_directoryResolver.GetUserDirectory(directory, _user), fileName);
            
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
            return Directory.GetFiles(_directoryResolver.GetUserDirectory(directory, _user));
        }

        public void DeleteFiles(FileDirectory directory, params string[] fileNames)
        {
            foreach (string fileName in fileNames)
                deleteFile(directory, fileName);
        }

        public bool FileExist(FileDirectory directory, string fileName)
        {
            string path = Path.Combine(_directoryResolver.GetUserDirectory(directory, _user), fileName);
            return File.Exists(path);
        }

        public FileInfo GetFileInfo(FileDirectory directory, string fileName)
        {
            string path = Path.Combine(_directoryResolver.GetUserDirectory(directory, _user), fileName);
            return new FileInfo(path);
        }

        public void MoveFile(FileDirectory currentDirectory, FileDirectory newDirectory, string fileName)
        {
            string currentFilePath = Path.Combine(_directoryResolver.GetUserDirectory(currentDirectory, _user), fileName);
            string newFilePath = Path.Combine(_directoryResolver.GetUserDirectory(newDirectory, _user), fileName);

            File.Move(currentFilePath, newFilePath);
        }

        public string GenerateFileKey(FileDirectory directory, string fileName)
        {
            return FileKeyGenerator.GenerateKey(Path.Combine(_directoryResolver.GetUserDirectory(directory, _user), fileName));
        }

        #endregion IFileAccessor Memebers

        #region Private Members

        private bool saveFile(FileDirectory directory, FileStreamWrapper file, FileMode fileMode)
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
            string userDirectory = _directoryResolver.GetUserDirectory(directory, _user);
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

        #endregion Private Members
    }
}