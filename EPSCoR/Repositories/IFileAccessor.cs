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

        public static FileStreamWrapper FromHttpPostedFile(HttpPostedFileBase file)
        {
            return new FileStreamWrapper()
            {
                FileName = file.FileName,
                InputStream = file.InputStream
            };
        }

        public static FileStreamWrapper FromFineUpload(FineUpload file)
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
        MemoryStream OpenFile(string fileName);
        IEnumerable<string> GetFiles();
        void DeleteFiles(params string[] fileNames);
        bool FileExist(string fileName);
    }
}