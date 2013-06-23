using System.Collections.Generic;
using System.IO;
using System.Web;
using EPSCoR.Controllers;

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
                InputStream = file.InputStream
            };
        }
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
        bool SaveFiles(params FileStreamWrapper[] files);

        /// <summary>
        /// Deletes one or more files from the disk.
        /// </summary>
        /// <param name="fileNames">Names of the files to delete.</param>
        void DeleteFiles(params string[] fileNames);
        
        /// <summary>
        /// Returns a read-only file stream for the specified file. If the file does not exist will return null.
        /// </summary>
        /// <param name="fileName">File to open.</param>
        /// <returns></returns>
        FileStream OpenFile(string fileName);
        
        /// <summary>
        /// Returns a list of all the files in the directory.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetFiles();

        /// <summary>
        /// Returns the file info on the specified file. Will return null if the file does not exist.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        FileInfo GetFileInfo(string fileName);
        
        /// <summary>
        /// Returns true if the file exisit.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool FileExist(string fileName);
    }
}