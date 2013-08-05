using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using EPSCoR.Database;
using EPSCoR.Database.Models;

namespace EPSCoR.Repositories.Basic
{
    /// <summary>
    /// This implementation uses a DbContext to access the database
    /// </summary>
    /// <typeparam name="T">Model type</typeparam>
    public class BasicModelRepo<T> : IModelRepository<T>
        where T : class, IModel
    {
        private ModelDbContext _context;

        public BasicModelRepo()
        {
            _context = new ModelDbContext();
        }

        public T Get(int entityID)
        {
            return _context.Set<T>().Find(entityID);
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>().AsQueryable();
        }

        public void Create(T itemToCreate)
        {
            _context.CreateModel(itemToCreate);
        }

        public void Update(T itemToUpdate)
        {
            _context.UpdateModel(itemToUpdate);
        }

        public void Remove(int entityID)
        {
            T itemToRemove = Get(entityID);
            _context.RemoveModel(itemToRemove);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}