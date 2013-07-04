using EPSCoR.Database.Models;
using EPSCoR.Database.Services;
using EPSCoR.Repositories.Basic;
using EPSCoR.Repositories.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.Repositories.Factory
{
    public class RepositoryFactory
    {
        public static IFileAccessor GetFileAccessor(string userName)
        {
            return new BasicFileAccessor(userName);
        }

        public static IAsyncFileAccessor GetAsyncFileAccessor(string userName)
        {
            return new AsyncFileAccessor(userName);
        }

        public static IModelRepository<T> GetModelRepository<T>()
            where T : class, IModel
        {
            return new BasicModelRepo<T>();
        }

        public static IAsyncModelRepository<T> GetAsyncModelRepository<T>()
            where T : class, IModel
        {
            return new AsyncModelRepo<T>();
        }

        public static ITableRepository GetTableRepository(string userName)
        {
            return new BasicTableRepo(userName);
        }

        public static IAsyncTableRepository GetAsyncTableRepository(string userName)
        {
            return new AsyncTableRepo(userName);
        }

        public static IDatabaseCalc GetDatabaseCalc(string userName)
        {
            return new BasicTableRepo(userName);
        }

        public static IAsyncDatabaseCalc GetAsyncDatabaseCalc(string userName)
        {
            return new AsyncTableRepo(userName);
        }
    }
}