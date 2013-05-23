using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSCoR.Database.Services.Log
{
    public class LoggerFactory
    {
        public static ILogger Logger = new FileLogger();
    }
}
