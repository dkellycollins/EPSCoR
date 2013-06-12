using System.IO;
using EPSCoR.Database.Exceptions;

namespace EPSCoR.Database.Services.FileConverter
{
    public class FileConverterFactory
    {
        public static IFileConverter GetConverter(string file)
        {
            string ext = Path.GetExtension(file).ToLower();
            switch (ext)
            {
                case ".csv":
                    return new CSVFileConverter(file);
                default:
                    throw new InvalidFileException(file, "Unsuppoerted file type");
            }
        }
    }
}
