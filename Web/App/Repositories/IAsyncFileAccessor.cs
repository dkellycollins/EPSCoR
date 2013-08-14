using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EPSCoR.Web.App.Repositories
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
        Task<bool> SaveFilesAsync(FileDirectory directory, params FileStreamWrapper[] files);

        /// <summary>
        /// Deletes one or more files from the disk.
        /// </summary>
        /// <param name="fileNames">Names of the files to delete.</param>
        Task DeleteFilesAsync(FileDirectory directory, params string[] fileNames);

        /// <summary>
        /// Returns a read-only file stream for the specified file. If the file does not exist will return null.
        /// </summary>
        /// <param name="fileName">File to open.</param>
        /// <returns></returns>
        Task<FileStream> OpenFileAsync(FileDirectory directory, string fileName);

        /// <summary>
        /// Returns a list of all the files in the directory.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<string>> GetFilesAsync(FileDirectory directory);

        /// <summary>
        /// Returns the file info on the specified file. Will return null if the file does not exist.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<FileInfo> GetFileInfoAsync(FileDirectory directory, string fileName);

        /// <summary>
        /// Returns true if the file exisit.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<bool> FileExistAsync(FileDirectory directory, string fileName);
        
        /// <summary>
        /// Moves the file from the current directory to the new directory.
        /// </summary>
        /// <param name="currentDirectory">The directory that the file is currently in.</param>
        /// <param name="newDirectory">The directory to move the file too.</param>
        /// <param name="fileName">Name of the file to move.</param>
        Task MoveFileAsync(FileDirectory currentDirectory, FileDirectory newDirectory, string fileName);

        Task<string> GenerateFileKeyAsync(FileDirectory directory, string fileName);
    }
}