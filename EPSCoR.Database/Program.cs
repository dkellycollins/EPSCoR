using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSCoR.Database
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                startFileProcessor(args);
                return;
            }
            if (args.Length == 1)
            {
                printUsage();
                return;
            }

            string action = args[1];
            switch (action.ToLower())
            {
                case "start":
                    startFileProcessor(args);
                    break;
                case "stop":
                    stopFileProcessor(args);
                    break;
                case "process":
                    processFile(args);
                    break;
                default:
                    printUsage();
                    break;
            }
        }

        private static void processFile(string[] args)
        {
            List<string> Params = args.ToList();
            string filePath = null;
            string tableName = null;
            string userName = null;
            int index = 0;

            if (!args.Contains("-f"))
            {
                printUsage();
                return;
            }
            else
            {
                index = Params.IndexOf("-f") + 1;
                filePath = Params[index];
            }

            if (args.Contains("-t"))
            {
                index = Params.IndexOf("-t") + 1;
                tableName = Params[index];
            }

            if (args.Contains("-u"))
            {
                index = Params.IndexOf("-u") + 1;
                userName = Params[index];
            }

            FileProcessor.ProcessFile(filePath, tableName, userName);
        }

        private static void stopFileProcessor(string[] args)
        {
            FileProcessor.Dispose();
        }

        private static void startFileProcessor(string[] args)
        {
            List<string> Params = new List<string>(args);
            string dataDirectory = null;

            if (!Params.Contains("-d"))
            {
                printUsage();
                return;
            }
            else
            {
                int index = Params.IndexOf("-d") + 1;
                dataDirectory = Params[index];
            }

            FileProcessor.Init(dataDirectory);
        }

        private static void printUsage()
        {
            Console.Out.WriteLine("Working on it");
        }
    }
}
