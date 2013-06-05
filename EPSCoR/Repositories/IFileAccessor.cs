using System.Collections.Generic;
using System.IO;
using System.Web;
using EPSCoR.Controllers;

namespace EPSCoR.Repositories
{
    public struct FileStreamWrapper
    {
        public string FileName;
        public Stream InputStream;
        public int SeekPos;
        public int FileSize;

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
        bool SaveFiles(params FileStreamWrapper[] files);

        void DeleteFiles(params string[] fileNames);
        
        FileStream OpenFile(string fileName);
        void CloseFile(FileStream fileStream);
        
        IEnumerable<string> GetFiles();
        FileInfo GetFileInfo(string fileName);
        bool FileExist(string fileName);
    }
}