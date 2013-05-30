﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSCoR.Database.Services.Log
{
    public class ConsoleLogger : ILogger
    {
        public ConsoleLogger()
        {
        }

        public void Log(string message)
        {
            Console.Out.WriteLine(message);
        }

        public void Log(string message, Exception e)
        {
            Log(string.Format("{0} : {1}", message, e.Message));
        }
    }
}