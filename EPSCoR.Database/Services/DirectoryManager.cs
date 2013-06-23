using System.IO;
using System.Web;

namespace EPSCoR.Database.Services
{
    /// <summary>
    /// Stores the directories the database uses. Also ensures each directory actually exist.
    /// </summary>
    public class DirectoryManager
    {
        /// <summary>
        /// Maps the root directory of the server.
        /// </summary>
        /// <param name="server"></param>
        public static void Initialize(HttpServerUtility server)
        {
            RootDir = server.MapPath("~/App_Data");
        }

        public static string RootDir = string.Empty;

        /// <summary>
        /// The directory where uploaded files should be stored.
        /// </summary>
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

        /// <summary>
        /// The directory where the results of a conversion should be stored.
        /// </summary>
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

        /// <summary>
        /// The directory where the processed files should be stored.
        /// </summary>
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

        /// <summary>
        /// The direcotry where files that threw an exception should be stored.
        /// </summary>
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

        /// <summary>
        /// The directory where temp files shoudl be stored.
        /// </summary>
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
