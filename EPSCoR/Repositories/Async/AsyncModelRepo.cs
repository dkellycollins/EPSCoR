using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using EPSCoR.Database.Models;
using EPSCoR.Database;
using System.Threading.Tasks;

namespace EPSCoR.Repositories.Async
{
    /// <summary>
    /// This implementation uses a DbContext to access the database
    /// </summary>
    /// <typeparam name="T">Model type</typeparam>
    public class AsyncModelRepo<T> : IAsyncModelRepository<T>
        where T : class, IModel
    {
        private ModelDbContext _context;

        public AsyncModelRepo()
        {
            _context = new ModelDbContext();
        }

        #region IAsyncModelRepository Members

        public async Task<T> GetAsync(int entityID)
        {
            return await Task.Run(() => _context.Set<T>().Find(entityID));
        }

        public async Task<IQueryable<T>> GetAllAsync()
        {
            return await Task.Run(() => _context.Set<T>().AsQueryable());
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