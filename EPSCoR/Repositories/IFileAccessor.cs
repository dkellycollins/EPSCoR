using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EPSCoR.Repositories
{
    /// <summary>
    /// Interface for using the file system on a data base.
    /// </summary>
    public interface IFileAccessor
    {
        bool SaveFile(HttpPostedFile file);
        bool SaveFile(FileStream file);
        FileStream OpenFile(string fileName);
        IEnumerable<string> GetFiles();
        void DeleteFile(string fileName);
        bool FileExist();
    }
}