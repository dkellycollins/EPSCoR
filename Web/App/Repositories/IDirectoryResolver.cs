using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using EPSCoR.Web.Database.Services;

namespace EPSCoR.Web.App.Repositories
{
    public interface IDirectoryResolver
    {
        /// <summary>
        /// Determines the path to the user directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        string GetUserDirectory(FileDirectory directory, string user);
    }

    public class DirectoryResolver : IDirectoryResolver
    {
        public DirectoryResolver()
        { }

        public string GetUserDirectory(FileDirectory directory, string user)
        {
            string userDirectory;
            switch (directory)
            {
                case FileDirectory.Archive:
                    userDirectory = Path.Combine(DirectoryManager.ArchiveDir, user);
                    break;
                case FileDirectory.Conversion:
                    userDirectory = Path.Combine(DirectoryManager.ConversionDir, user);
                    break;
                case FileDirectory.Invalid:
                    userDirectory = Path.Combine(DirectoryManager.InvalidDir, user);
                    break;
                case FileDirectory.Temp:
                    userDirectory = Path.Combine(DirectoryManager.TempDir, user);
                    break;
                case FileDirectory.Upload:
                    userDirectory = Path.Combine(DirectoryManager.UploadDir, user);
                    break;
                default:
                    throw new Exception("Unknown Directory");
            }

            if (!Directory.Exists(userDirectory))
                Directory.CreateDirectory(userDirectory);

            return userDirectory;
        }
    }
}