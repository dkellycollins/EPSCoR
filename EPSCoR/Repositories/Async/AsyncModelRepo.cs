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
    public class AsyncModelRepo<T> : IModelRepository<T>, IAsyncModelRepository<T>
        where T : class, IModel
    {
        private DbContext _context;

        public AsyncModelRepo()
        {
            _context = new DefaultContext();
        }

        public AsyncModelRepo(DbContext context)
        {
            _context = context;
        }

        public T Get(int entityID)
        {
            return GetAsync(entityID).Result;
        }

        public async Task<T> GetAsync(int entityID)
        {
            return await Task.Run(() => _context.Set<T>().Find(entityID));
        }

        public IQueryable<T> GetAll()
        {
            return GetAllAsync().Result;
        }

        public async Task<IQueryable<T>> GetAllAsync()
        {
            return await Task.Run(() => _context.Set<T>().AsQueryable());
        }

        public void Create(T itemToCreate)
        {
            itemToCreate.DateCreated = DateTime.Now;
            itemToCreate.DateUpdated = DateTime.Now;
            CreateAsync(itemToCreate);
        }

        public async void CreateAsync(T itemToCreate)
        {
            await Task.Run(() =>
            {
                _context.Set<T>().Add(itemToCreate);
                _context.SaveChanges();
            });
        }

        public void Update(T itemToUpdate)
        {
            itemToUpdate.DateUpdated = DateTime.Now;
            UpdateAsync(itemToUpdate);
        }

        public async void UpdateAsync(T itemToUpdate)
        {
            await Task.Run(() =>
            {
                _context.Entry(itemToUpdate).State = System.Data.EntityState.Modified;
                _context.SaveChanges();
            });
        }

        public void Remove(int entityID)
        {
            RemoveAsync(entityID);
        }

        public async void RemoveAsync(int entityID)
        {
            T itemToRemove = await GetAsync(entityID);
            await Task.Run(() =>
            {
                _context.Set<T>().Remove(itemToRemove);
                _context.SaveChanges();
            });
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}