using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using EPSCoR.Database.Models;

namespace EPSCoR.Repositories
{
    public class CachedRepo<T> : IModelRepository<T> 
        where T : class, IModel 
    {
        private DbContext _dbContext;

        public CachedRepo()
        {
            _dbContext = DefaultContext.GetInstance();
        }

        public T Get(int entityID)
        {
            HttpContext httpContext = HttpContext.Current;
            string key = typeof(T).Namespace + "-" + entityID;

            if (httpContext.Cache[key] == null)
            {
                httpContext.Cache.Add(
                    key,
                    _dbContext.Set<T>().Find(entityID),
                    null,
                    Cache.NoAbsoluteExpiration,
                    new TimeSpan(0, 20, 0),
                    CacheItemPriority.Default,
                    null);
            }

            return (T)httpContext.Cache[key];
        }

        public IQueryable<T> GetAll()
        {
            HttpContext httpContext = HttpContext.Current;
            string key = typeof(T).Namespace + "-all";

            if (httpContext.Cache[key] == null)
            {
                httpContext.Cache.Add(
                    key,
                    _dbContext.Set<T>().AsQueryable(),
                    null,
                    Cache.NoAbsoluteExpiration,
                    new TimeSpan(0, 20, 0),
                    CacheItemPriority.Default,
                    null);
            }

            return (IQueryable<T>)httpContext.Cache[key];
        }

        public void Create(T itemToCreate)
        {
            throw new NotImplementedException();
        }

        public void Update(T itemToUpdate)
        {
            throw new NotImplementedException();
        }

        public void Remove(int entityID)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            DefaultContext.Release();
        }
    }
}
