using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using EPSCoR.Database.Services;
using EPSCoR.ViewModels;

namespace EPSCoR.Repositories
{
    /// <summary>
    /// Wraps the file stream and other info for the file.
    /// </summary>
    public struct FileStreamWrapper
    {
        /// <summary>
        /// Name of the file.
        /// </summary>
        public string FileName;

        /// <summary>
        /// Stream for the file.
        /// </summary>
        public Stream InputStream;

        /// <summary>
        /// Where the stream starts in the complete file.
        /// </summary>
        public int SeekPos;

        /// <summary>
        /// The size of the complete file.
        /// </summary>
        public int FileSize;

        /// <summary>
        /// Returns a FileStreamWrapper with fields populated from the HttpPostedFileBase.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static FileStreamWrapper FromHttpPostedFile(HttpPostedFileBase file)
        {
            return new FileStreamWrapper()
            {
                FileName = file.FileName,
                InputStream = file.InputStream,
                SeekPos = 0,
                FileSize = (int)file.InputStream.Length
            };
        }

        /// <summary>
        /// Retursn a FileStreamWrapper with populated from the FileUpload.
        /// </summary>
        /// <param name="fileUpload"></param>
        /// <returns></returns>
        public static FileStreamWrapper FromFileUpload(FileUpload fileUpload)
        {
            return new FileStreamWrapper()
            {
                FileName = fileUpload.FileName,
                InputStream = fileUpload.InputStream,
                SeekPos = fileUpload.StartPosition,
                FileSize = fileUpload.TotalFileLength
            };
        }
    }

    public enum FileDirectory
    {
        Temp,
        Upload,
        Conversion,
        Archive,
        Invalid
    }

    /// <summary>
    /// Interface for using the file system on a data base.
    /// </summary>
    public interface IFileAccessor
    {
        /// <summary>
        /// Saves one or more files to disk.
        /// </summary>
        /// <param name="files">The files to save.</param>
        /// <returns>True if each save was successful.</returns>
        bool SaveFiles(FileDirectory directory, params FileStreamWrapper[] files);

        /// <summary>
        /// Deletes one or more files from the disk.
        /// </summary>
        /// <param name="fileNames">Names of the files to delete.</param>
        void DeleteFiles(FileDirectory directory, params string[] fileNames);
        
        /// <summary>
        /// Returns a read-only file stream for the specified file. If the file does not exist will return null.
        /// </summary>
        /// <param name="fileName">File to open.</param>
        /// <returns></returns>
        FileStream OpenFile(FileDirectory directory, string fileName);
        
        /// <summary>
        /// Returns a list of all the files in the directory.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetFiles(FileDirectory directory);

        /// <summary>
        /// Returns the file info on the specified file. Will return null if the file does not exist.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        FileInfo GetFileInfo(FileDirectory directory, string fileName);
        
        /// <summary>
        /// Returns true if the file exisit.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool FileExist(FileDirectory directory, string fileName);

        /// <summary>
        /// Moves the file from the current directory to the new directory.
        /// </summary>
        /// <param name="currentDirectory">The directory that the file is currently in.</param>
        /// <param name="newDirectory">The directory to move the file too.</param>
        /// <param name="fileName">Name of the file to move.</param>
        void MoveFile(FileDirectory currentDirectory, FileDirectory newDirectory, string fileName);

        string GenerateFileKey(FileDirectory directory, string fileName);
    }
}