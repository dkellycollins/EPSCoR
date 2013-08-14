using EPSCoR.Web.App.Repositories.Async;
using EPSCoR.Web.App.Repositories.Basic;
using EPSCoR.Web.Database.Models;

namespace EPSCoR.Web.App.Repositories.Factory
{
    public class RepositoryFactory
    {
        public static IFileAccessor GetFileAccessor(string userName)
        {
            return new BasicFileAccessor(userName, new DirectoryResolver());
        }

        public static IAsyncFileAccessor GetAsyncFileAccessor(string userName)
        {
            return new AsyncFileAccessor(userName, new DirectoryResolver());
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