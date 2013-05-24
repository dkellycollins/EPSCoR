using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSCoR.Database
{
    /// <summary>
    /// This file converter handles csv files.
    /// </summary>
    public class CSVFileConverter : IFileConverter
    {
        public string Convert(string file)
        {
            //CSV files do not need to be converted. Just return the same file.
            return file;
        }
    }
}
