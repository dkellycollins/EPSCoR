using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EPSCoR.Database.Services
{
    public class DirectoryManager
    {
        public static void Initialize(HttpServerUtility server)
        {
            RootDir = server.MapPath("~/App_Data");
        }

        public static string RootDir = string.Empty;

        public static string UploadDir
        {
            get
            {
                string dir = Path.Combine(RootDir, "Uploads");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return dir;
            }
        }

        public static string ConversionDir
        {
            get
            {
                string dir = Path.Combine(RootDir, "Conversions");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return dir;
            }
        }

        public static string ArchiveDir
        {
            get
            {
                string dir = Path.Combine(RootDir, "Archive");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return dir;
            }
        }

        public static string InvalidDir
        {
            get
            {
                string dir = Path.Combine(RootDir, "Invalid");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return dir;
            }
        }

        public static string TempDir
        {
            get
            {
                string dir = Path.Combine(RootDir, "Temp");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return dir;
            }
        }
    }
}
