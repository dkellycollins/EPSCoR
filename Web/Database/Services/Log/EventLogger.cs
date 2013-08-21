using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSCoR.Web.Database.Services.Log
{
    public class EventLogger : ILogger
    {
        private EventLog _eventLog;

        public EventLogger()
        {
            _eventLog = new EventLog();
        }

        public EventLogger(EventLog eventLog)
        {
            _eventLog = eventLog;
        }

        public void Log(string message)
        {
            _eventLog.WriteEntry(message, EventLogEntryType.Information);
        }

        public void Log(string message, Exception e)
        {
            _eventLog.WriteEntry(message + " " + e.ToString(), EventLogEntryType.Error);
        }
    }
}
