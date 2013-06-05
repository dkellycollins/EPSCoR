using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSCoR.Database.Services.FileConverter
{
    /// <summary>
    /// This file converter handles csv files.
    /// </summary>
    public class CSVFileConverter : IFileConverter
    {
        public string File { get; private set; }

        public CSVFileConverter(string file)
        {
            File = file;
        }

        public string ConvertToCSV()
        {
            //CSV files do not need to be converted. Just return the same file.
            return File;
        }
    }
}
