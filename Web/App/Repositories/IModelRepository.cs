using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EPSCoR.Web.Database.Models;

namespace EPSCoR.Web.App.Repositories
{
    /// <summary>
    /// Provides basic crud operations for a models in the database.
    /// </summary>
    /// <typeparam name="T">Model Type.</typeparam>
    public interface IModelRepository<T> : IDisposable
        where T : Model
    {
        /// <summary>
        /// Returns one entity that has the given id.
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns></returns>
        T Get(int entityID);

        /// <summary>
        /// Returns all entities in the database.
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Returns all entities that match the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Creates a new entity in the database.
        /// </summary>
        /// <param name="itemToCreate"></param>
        void Create(T itemToCreate);

        /// <summary>
        /// Updates an entity in the database with values in itemToUpdate.
        /// </summary>
        /// <param name="itemToUpdate"></param>
        void Update(T itemToUpdate);

        /// <summary>
        /// Removes an entity in the database that has the given id.
        /// </summary>
        /// <param name="entityID"></param>
        void Remove(int entityID);
    }
}