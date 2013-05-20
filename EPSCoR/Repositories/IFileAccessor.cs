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
        bool SaveFiles(params HttpPostedFileBase[] files);
        FileStream OpenFile(string fileName);
        IEnumerable<string> GetFiles();
        void DeleteFiles(params string[] fileNames);
        bool FileExist(string fileName);
    }
}