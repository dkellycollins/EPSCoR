using System.IO;

namespace EPSCoR.Database.Services
{
    public class FilePoll
    {
        private System.Timers.Timer _timer;

        public delegate void FileFoundEventHandler(string filePath);
        public event FileFoundEventHandler FileFound = delegate { };

        public string RootDirectory { get; set;}
        public bool IncludeSubDirectories { get; set; }
        public string Filter { get; set; }

        public FilePoll(int interval, string rootDirectory)
        {
            _timer = new System.Timers.Timer()
            {
                AutoReset = true,
                Enabled = true,
                Interval = interval
            };
            _timer.Elapsed += _timer_Elapsed;

            RootDirectory = rootDirectory;
            IncludeSubDirectories = true;
            Filter = string.Empty;
        }

        public void Dispose()
        {
            _timer.Enabled = false;
            _timer.Dispose();
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            scan(RootDirectory, IncludeSubDirectories, Filter);
        }

        private void scan(string rootDirectory, bool includeSubDirectories, string filter)
        {
            string[] files = Directory.GetFiles(rootDirectory, filter);
            foreach (string file in files)
            {
                FileFound(file);
            }

            if (includeSubDirectories)
            {
                string[] directories = Directory.GetDirectories(rootDirectory);
                foreach (string directory in directories)
                {
                    scan(directory, includeSubDirectories, filter);
                }
            }
        }
    }
}
