using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ESPCoR.Database
{
    public class FileProcessor
    {
        private static string RootDir = Server.MapPath("~/App_Data");
        private static string UploadDir = Server.MapPath("~/App_Data/Uploads");
        private static string ConversionDir = Server.MapPath("~/App_Data/Convertions");
        private static string ArchiveDir = Server.MapPath("~/App_Data/Archive");
        private static HttpServerUtility Server
        {
            get { return System.Web.HttpContext.Current.Server; }
        }

        private static bool _cancel;

        public static void Start()
        {
            _cancel = false;
            Thread t = new Thread(convertFiles);
            t.Start();
        }

        public static void Stop()
        {
            _cancel = true;
        }

        private static void convertFiles()
        {
            if (!Directory.Exists(UploadDir))
                Directory.CreateDirectory(UploadDir);
            if(!Directory.Exists(ConversionDir))
                Directory.CreateDirectory(ConversionDir);
            if(!Directory.Exists(ArchiveDir))
                Directory.CreateDirectory(ArchiveDir);

            while (!_cancel)
            {
                string lockFile = null;
                try
                {
                    foreach (string userDirectory in Directory.GetDirectories(UploadDir))
                    {
                        if (_cancel)
                            break;

                        string l = Path.Combine(userDirectory, "lock");
                        List<string> files = new List<string>(Directory.GetFiles(userDirectory));
                        if (files.Count() > 0 && !files.Contains(l))
                        {
                            lockFile = l;
                            File.Create(lockFile).Close();
                            files.Remove(lockFile);
                        }
                        else
                        {
                            break;
                        }

                        foreach (string file in files)
                        {
                            if (_cancel)
                                break;

                            //Convert the file.
                            string ext = Path.GetExtension(file).ToLower();
                            switch (ext)
                            {
                                case ".csv":
                                    handleCSV(file);
                                    break;
                                case ".dbf":
                                    handleDBF(file);
                                    break;
                                case ".mdb":
                                    handleMDB(file);
                                    break;
                                case ".xml":
                                    handleXML(file);
                                    break;
                            }

                            //Move the original file to the Archive.
                            string archivePath = Path.Combine(ArchiveDir, Directory.GetParent(file).Name, Path.GetFileName(file));
                            validateDestination(archivePath);
                            File.Move(file, archivePath);

                            Log("File processed: " + file);
                        }

                        File.Delete(lockFile);
                    }
                }
                catch (Exception e)
                {
                    Log("Exception: " + e.Message);
                }
                finally
                {
                    if (lockFile != null)
                        File.Delete(lockFile);
                }
            }
        }

        private static void handleXML(string file)
        {
            throw new NotImplementedException();
        }

        private static void handleMDB(string file)
        {
            throw new NotImplementedException();
        }

        private static void handleDBF(string file)
        {
            throw new NotImplementedException();
        }

        private static void handleCSV(string file)
        {
            //CSV files dont need any converstion. Just copy the file to the CnversionDirectory
            string userDir = Directory.GetParent(file).Name;
            string fileName = Path.GetFileName(file);
            string conversionPath = Path.Combine(ConversionDir, userDir, fileName);
            validateDestination(conversionPath);
            File.Copy(file, conversionPath);
        }

        private static void Log(string entry)
        {
            string logFile = Path.Combine(RootDir, "Log.txt");
            StreamWriter logFileStream;
            if (!File.Exists(logFile))
                logFileStream = File.CreateText(logFile);
            else
                logFileStream = new StreamWriter(File.Open(logFile, FileMode.Append));

            logFileStream.WriteLine(string.Format("{0} - {1}", DateTime.Now.ToString(), entry));

            logFileStream.Flush();
            logFileStream.Close();
        }

        private static void validateDestination(string dest)
        {
            string destDir = Directory.GetParent(dest).FullName;
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);
            if (File.Exists(dest))
                File.Delete(dest);
        }
    }
}
