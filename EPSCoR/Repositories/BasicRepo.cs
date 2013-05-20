using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using EPSCoR.Database.Models;

namespace EPSCoR.Repositories
{
    /// <summary>
    /// This implementation uses a DbContext to access the database
    /// </summary>
    /// <typeparam name="T">Model type</typeparam>
    public class BasicRepo<T> : IRepository<T>
        where T : class
    {
        private DbContext _context;

        public BasicRepo()
        {
            _context = DefaultContext.GetInstance();
        }

        public BasicRepo(DbContext context)
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
            _context.Set<T>().Add(itemToCreate);
            _context.SaveChanges();
        }

        public void Update(T itemToUpdate)
        {
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
            DefaultContext.Release();
        }
    }
}