using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace EPSCoR.Database.Services
{
    public class CompleteFileWatcher : FileSystemWatcher
    {
        private static TimeSpan TIMER_INTERVAL = new TimeSpan(0, 5, 0);

        private Timer _timer;
        private string _directory;

        public delegate void NewFileEventHandler(string fileName);

        public event NewFileEventHandler NewFile = delegate { };

        public CompleteFileWatcher(string directory)
            : base(directory)
        {
            _timer = new Timer()
            {
                AutoReset = true,
                Enabled = true,
                Interval = TIMER_INTERVAL.Milliseconds,
            };
            _timer.Elapsed += _timer_Elapsed;

            base.Created += (sender, e) => NewFile(e.FullPath);
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            checkForFiles(_directory);
            foreach (string directory in Directory.EnumerateDirectories(_directory))
                checkForFiles(directory);
        }

        private void checkForFiles(string directory)
        {
            foreach (string file in Directory.EnumerateFiles(directory))
            {
                NewFile(file);
            }
        }
    }
}
