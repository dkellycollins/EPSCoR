using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSCoR.Database
{
    public interface IFileConverter
    {
        string Convert(string file);
    }
}
