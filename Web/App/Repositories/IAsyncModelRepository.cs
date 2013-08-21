using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EPSCoR.Web.Database.Models;

namespace EPSCoR.Web.App.Repositories
{
    /// <summary>
    /// Provides basic crud operations for a models in the database.
    /// </summary>
    /// <typeparam name="T">Model Type.</typeparam>
    public interface IAsyncModelRepository<T> : IDisposable
        where T : Model
    {
        /// <summary>
        /// Returns one entity that has the given id.
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns></returns>
        Task<T> GetAsync(int entityID);

        /// <summary>
        /// Returns all entities in the database.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Creates a new entity in the database.
        /// </summary>
        /// <param name="itemToCreate"></param>
        Task CreateAsync(T itemToCreate);

        /// <summary>
        /// Updates an entity in the database with values in itemToUpdate.
        /// </summary>
        /// <param name="itemToUpdate"></param>
        Task UpdateAsync(T itemToUpdate);

        /// <summary>
        /// Removes an entity in the database that has the given id.
        /// </summary>
        /// <param name="entityID"></param>
        Task RemoveAsync(int entityID);
    }
}