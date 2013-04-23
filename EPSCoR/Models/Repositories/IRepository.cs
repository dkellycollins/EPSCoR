using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.Models.Repositories
{
    /// <summary>
    /// Interface for accessing the database.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> : IDisposable
        where T : class
    {
        T Get(int entityID);

        IQueryable<T> GetAll();

        void Create(T itemToCreate);

        void Update(T itemToUpdate);

        void Remove(int entityID);
    }
}