using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EPSCoR.Repositories
{
    /// <summary>
    /// Interface for using the file system on a data base.
    /// </summary>
    public interface IAsyncFileAccessor
    {
        /// <summary>
        /// Saves one or more files to disk.
        /// </summary>
        /// <param name="files">The files to save.</param>
        /// <returns>True if each save was successful.</returns>
        Task<bool> SaveFilesAsync(params FileStreamWrapper[] files);

        /// <summary>
        /// Deletes one or more files from the disk.
        /// </summary>
        /// <param name="fileNames">Names of the files to delete.</param>
        void DeleteFilesAsync(params string[] fileNames);

        /// <summary>
        /// Returns a read-only file stream for the specified file. If the file does not exist will return null.
        /// </summary>
        /// <param name="fileName">File to open.</param>
        /// <returns></returns>
        Task<FileStream> OpenFileAsync(string fileName);

        /// <summary>
        /// Returns a list of all the files in the directory.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> GetFilesAsync();

        /// <summary>
        /// Returns the file info on the specified file. Will return null if the file does not exist.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<FileInfo> GetFileInfoAsync(string fileName);

        /// <summary>
        /// Returns true if the file exisit.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<bool> FileExistAsync(string fileName);
    }
}