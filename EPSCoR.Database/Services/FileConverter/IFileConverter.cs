using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSCoR.Database.Services.FileConverter
{
    public interface IFileConverter
    {
        string ConvertToCSV();
    }
}
