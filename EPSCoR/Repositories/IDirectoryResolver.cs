﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using EPSCoR.Database.Services;

namespace EPSCoR.Repositories
{
    public interface IDirectoryResolver
    {
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