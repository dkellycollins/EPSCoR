using System;
using System.IO;
using EPSCoR.Web.Database.Services;
using EPSCoR.Web.Database.Services.Log;

namespace EPSCoR.Web.FileProcessor.FileConverter
{
    /// <summary>
    /// This file converter handles csv files.
    /// </summary>
    public class CSVFileConverter : IFileConverter
    {
        public string FilePath { get; private set; }
        public string User { get; private set; }

        public CSVFileConverter(string file, string user)
        {
            FilePath = file;
            User = user;
        }

        public string ConvertToCSV()
        {
            //Ensure that the contents of the csv can be used as doubles. Remove any data that connot be used.
            string userDirectory = Path.Combine(DirectoryManager.ConversionDir, User);
            if (!Directory.Exists(userDirectory))
                Directory.CreateDirectory(userDirectory);
            string processedFile = Path.Combine(userDirectory, Path.GetFileName(FilePath));

            LoggerFactory.GetLogger().Log("Begining to validate file: " + FilePath);

            using (TextReader reader = File.OpenText(FilePath))
            {
                using (TextWriter writer = File.CreateText(processedFile))
                {
                    //First line should be the headers, dont need to modify those.
                    writer.WriteLine(reader.ReadLine());

                    string buf;
                    while ((buf = reader.ReadLine()) != null)
                    {
                        string[] values = buf.Split(',');
                        foreach (string value in values)
                        {
                            double x;
                            if (double.TryParse(value, out x))
                            {
                                writer.Write(x + ", ");
                            }
                            else if (TryParseSci(value, out x))
                            {
                                writer.Write(x + ", ");
                            }
                            else
                            {
                                writer.Write(0.0 + ", ");
                            }
                        }
                        writer.WriteLine();
                    }

                    writer.Flush();
                }
            }

            LoggerFactory.GetLogger().Log("Validated file: " + FilePath);

            return processedFile;
        }

        /// <summary>
        /// Attempts to parse a number that is in scientific notation.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <param name="result">If the convertion is successfull then the result will be stored here. Otherwise this will equal 0.</param>
        /// <returns>True if the conversion was successfull.</returns>
        private bool TryParseSci(string value, out double result)
        {
            try
            {
                int eIndex = value.IndexOf('e');

                char sign = value[eIndex + 1];
                string baseValue = value.Substring(0, value.Length - eIndex);
                string modValue = value.Substring(eIndex + 2);
                double a = double.Parse(baseValue);
                double b = double.Parse(modValue);

                switch (sign)
                {
                    case '+':
                        result = a * b;
                        break;
                    case '-':
                        result = a / b;
                        break;
                    default:
                        throw new Exception();
                }
                return true;
            }
            catch
            {
                result = 0;
                return false;
            }
        }
    }
}
