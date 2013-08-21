using EPSCoR.Web.App.Repositories.Async;
using EPSCoR.Web.App.Repositories.Basic;
using EPSCoR.Web.Database.Models;

namespace EPSCoR.Web.App.Repositories.Factory
{
    public interface IRepositoryFactory
    {
        IFileAccessor GetFileAccessor(string userName);
        IAsyncFileAccessor GetAsyncFileAccessor(string userName);
        IModelRepository<T> GetModelRepository<T>()
            where T: Model;
        IAsyncModelRepository<T> GetAsyncModelRepository<T>()
            where T: Model;
        ITableRepository GetTableRepository(string userName);
        IAsyncTableRepository GetAsyncTableRepository(string userName);
        IDatabaseCalc GetDatabaseCalc(string userName);
        IAsyncDatabaseCalc GetAsyncDatabaseCalc(string userName);
    }

    public class RepositoryFactory : IRepositoryFactory
    {
        public IFileAccessor GetFileAccessor(string userName)
        {
            return new BasicFileAccessor(userName, new DirectoryResolver());
        }

        public IAsyncFileAccessor GetAsyncFileAccessor(string userName)
        {
            return new AsyncFileAccessor(userName, new DirectoryResolver());
        }

        public IModelRepository<T> GetModelRepository<T>()
            where T : Model
        {
            return new BasicModelRepo<T>();
        }

        public IAsyncModelRepository<T> GetAsyncModelRepository<T>()
            where T : Model
        {
            return new AsyncModelRepo<T>();
        }

        public ITableRepository GetTableRepository(string userName)
        {
            return new BasicTableRepo(userName);
        }

        public IAsyncTableRepository GetAsyncTableRepository(string userName)
        {
            return new AsyncTableRepo(userName);
        }

        public IDatabaseCalc GetDatabaseCalc(string userName)
        {
            return new BasicTableRepo(userName);
        }

        public IAsyncDatabaseCalc GetAsyncDatabaseCalc(string userName)
        {
            return new AsyncTableRepo(userName);
        }
    }
}