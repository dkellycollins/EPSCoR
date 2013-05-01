using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using EPSCoR.Repositories;

namespace EPSCoR.Controllers
{
    /// <summary>
    /// Hanldes converting uploaded files.
    /// </summary>
    public class FileConverter
    {
        public static bool Convert(
            IFileAccessor uploadedFileAccessor,
            IFileAccessor convertionFileAccessor,
            IFileAccessor archiveFileAccessor,
            string fileName)
        {
            return false;
        }
    }
}