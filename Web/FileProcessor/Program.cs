using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace EPSCoR.Web.FileProcessor
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun = new ServiceBase[] 
            { 
                new FileProcessorService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
