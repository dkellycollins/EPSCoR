using EPSCoR.Database.Models;
using EPSCoR.Database.Services;
using EPSCoR.Repositories.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.Repositories.Factory
{
    public class RepositoryFactory
    {
        public static IFileAccessor GetConvertionFileAccessor(string userName)
        {
            return new BasicFileAccessor(DirectoryManager.ConversionDir, userName);
        }

        public static IFileAccessor GetUploadFileAccessor(string userName)
        {
            return new BasicFileAccessor(DirectoryManager.UploadDir, userName);
        }

        public static IFileAccessor GetTempFileAccessor(string userName)
        {
            return new BasicFileAccessor(DirectoryManager.TempDir, userName);
        }

        public static IModelRepository<T> GetModelRepository<T>()
            where T : class, IModel
        {
            return new BasicModelRepo<T>();
        }

        public static ITableRepository GetTableRepository(string userName)
        {
            return new BasicTableRepo(userName);
        }

        public static IDatabaseCalc GetDatabaseCalc(string userName)
        {
            return new BasicTableRepo(userName);
        }
    }
}