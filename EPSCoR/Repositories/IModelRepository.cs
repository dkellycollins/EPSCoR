using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPSCoR.Database.Models;

namespace EPSCoR.Repositories
{
    /// <summary>
    /// Interface for a repository that 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IModelRepository<T> : IDisposable
        where T : IModel
    {
        T Get(int entityID);

        IQueryable<T> GetAll();

        void Create(T itemToCreate);

        void Update(T itemToUpdate);

        void Remove(int entityID);
    }
}