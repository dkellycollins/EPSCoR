using System;

namespace EPSCoR.Database.Services.Log
{
    public interface ILogger
    {
        void Log(string message);
        void Log(string message, Exception e);
    }
}
