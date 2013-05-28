using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPSCoR.Database.Exceptions;

namespace EPSCoR.Database
{
    /// <summary>
    /// The generic file converter. This converter will call the correct convert based on the extension of the file.
    /// </summary>
    public class FileConverter : IFileConverter
    {
        /// <summary>
        /// Cached reference to the last converter used.
        /// </summary>
        private static IFileConverter _converter;

        /// <summary>
        /// Calls the correct file converter based on the extension of the file. If no converter can be found then an InvalidFileException is thrown.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string Convert(string file)
        {
            string ext = Path.GetExtension(file).ToLower();
            switch (ext)
            {
                case ".csv":
                    if (_converter == null || !(_converter is CSVFileConverter))
                        _converter = new CSVFileConverter();
                    break;
                default:
                    throw new InvalidFileException(file, "Unsupported file type");
            }

            return _converter.Convert(file);
        }
    }
}
