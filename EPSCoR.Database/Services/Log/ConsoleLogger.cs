﻿using System;

namespace EPSCoR.Database.Services.Log
{
    /// <summary>
    /// Prints message to the console.
    /// </summary>
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
