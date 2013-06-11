using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
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
        private DbContext _context;

        public BasicModelRepo()
        {
            _context = new DefaultContext();
        }

        public BasicModelRepo(DbContext context)
        {
            _context = context;
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
            itemToCreate.DateCreated = DateTime.Now;
            itemToCreate.DateUpdated = DateTime.Now;
            _context.Set<T>().Add(itemToCreate);
            _context.SaveChanges();
        }

        public void Update(T itemToUpdate)
        {
            itemToUpdate.DateUpdated = DateTime.Now;
            _context.Entry(itemToUpdate).State = System.Data.EntityState.Modified;
            _context.SaveChanges();
        }

        public void Remove(int entityID)
        {
            T itemToRemove = Get(entityID);
            _context.Set<T>().Remove(itemToRemove);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}