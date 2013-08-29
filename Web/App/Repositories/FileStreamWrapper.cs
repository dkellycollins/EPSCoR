using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using EPSCoR.Web.App.ViewModels;

namespace EPSCoR.Web.App.Repositories
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
}