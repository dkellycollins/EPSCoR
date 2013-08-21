using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EPSCoR.Web.Database;
using EPSCoR.Web.Database.Context;
using EPSCoR.Web.Database.Models;

namespace EPSCoR.Web.App.Repositories.Async
{
    /// <summary>
    /// This implementation uses a DbContext to access the database
    /// </summary>
    /// <typeparam name="T">Model type</typeparam>
    public class AsyncModelRepo<T> : IAsyncModelRepository<T>
        where T : Model
    {
        private ModelDbContext _context;

        public AsyncModelRepo()
        {
            IDbContextFactory contextFactory = new DbContextFactory();
            _context = contextFactory.GetModelDbContext();
        }

        public AsyncModelRepo(ModelDbContext context)
        {
            _context = context;
        }

        #region IAsyncModelRepository Members

        public async Task<T> GetAsync(int entityID)
        {
            return await Task.Run(() => _context.GetModel<T>(entityID));
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Task.Run(() => _context.GetAllModels<T>().ToList());
        }

        public async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.Run(() => _context.GetAllModels<T>().Where(predicate).ToList());
        }

        public async Task CreateAsync(T itemToCreate)
        {
            await Task.Run(() =>
            {
                _context.CreateModel(itemToCreate);
            });
        }

        public async Task UpdateAsync(T itemToUpdate)
        {
            await Task.Run(() =>
            {
                _context.UpdateModel(itemToUpdate);
            });
        }

        public async Task RemoveAsync(int entityID)
        {
            T itemToRemove = await GetAsync(entityID);
            await Task.Run(() =>
            {
                _context.RemoveModel(itemToRemove);
            });
        }

        #endregion IAsyncModelRepository Members

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}