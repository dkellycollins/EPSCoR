using System.IO;
using EPSCoR.Common.Exceptions;

namespace EPSCoR.Web.Database.Services.FileConverter
{
    public class FileConverterFactory
    {
        /// <summary>
        /// Get the correct file converted based on the extension of the file. if the file extension has no converter then an InvalidFileException is thrown.
        /// </summary>
        /// <param name="file">Full name of the file.</param>
        /// <param name="user">The name o fthe user who uploaded the file.</param>
        /// <returns></returns>
        public static IFileConverter GetConverter(string file, string user)
        {
            string ext = Path.GetExtension(file).ToLower();
            switch (ext)
            {
                case ".csv":
                    return new CSVFileConverter(file, user);
                default:
                    throw new InvalidFileException(file, "Unsuppoerted file type");
            }
        }
    }
}
