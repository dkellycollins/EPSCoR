﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using EPSCoR.Database.Models;

namespace EPSCoR.Repositories
{
    /// <summary>
    /// Provides basic crud operations for a models in the database.
    /// </summary>
    /// <typeparam name="T">Model Type.</typeparam>
    public interface IAsyncModelRepository<T> : IDisposable
        where T : IModel
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
        Task<IQueryable<T>> GetAllAsync();

        /// <summary>
        /// Creates a new entity in the database.
        /// </summary>
        /// <param name="itemToCreate"></param>
        void CreateAsync(T itemToCreate);

        /// <summary>
        /// Updates an entity in the database with values in itemToUpdate.
        /// </summary>
        /// <param name="itemToUpdate"></param>
        void UpdateAsync(T itemToUpdate);

        /// <summary>
        /// Removes an entity in the database that has the given id.
        /// </summary>
        /// <param name="entityID"></param>
        void RemoveAsync(int entityID);
    }
}