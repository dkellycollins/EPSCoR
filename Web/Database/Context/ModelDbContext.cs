using System;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using EPSCoR.Web.Database.Models;

namespace EPSCoR.Web.Database.Context
{
    /// <summary>
    /// Extends DbContext to allow listeners to be notified when a model has changed in the database.
    /// </summary>
    public class ModelDbContext : DbContext
    {
        public delegate void ModelEventHandler(IModel model);

        /// <summary>
        /// Raised when a model has been created in the database.
        /// </summary>
        public static event ModelEventHandler ModelCreated = delegate { };

        /// <summary>
        /// Raised when a model has been updated in the database.
        /// </summary>
        public static event ModelEventHandler ModelUpdated = delegate { };

        /// <summary>
        /// Raised when a model has been removed from the datbase.
        /// </summary>
        public static event ModelEventHandler ModelRemoved = delegate { };

        /// <summary>
        /// Used for testing purposes only.
        /// </summary>
        public ModelDbContext()
        { }

        public ModelDbContext(string connectionString)
            : base(connectionString)
        { }

        public ModelDbContext(DbConnection connection)
            : base(connection, false)
        { }

        /// <summary>
        /// Gets a single model from the database.
        /// </summary>
        /// <typeparam name="T">Type of the model.</typeparam>
        /// <param name="modelId">ID of the model.</param>
        /// <returns>The model.</returns>
        public virtual T GetModel<T>(int modelId)
            where T : class, IModel
        {
            return Set<T>().Find(modelId);
        }

        /// <summary>
        /// Gets all models from the database of type T.
        /// </summary>
        /// <typeparam name="T">Type of the model.</typeparam>
        /// <returns></returns>
        public virtual IQueryable<T> GetAllModels<T>()
            where T : class, IModel
        {
            return Set<T>().AsQueryable();
        }

        /// <summary>
        /// Creates a new model in the database.
        /// </summary>
        /// <param name="model">The model to create.</param>
        public virtual void CreateModel(IModel model)
        {
            model.DateCreated = DateTime.Now;
            model.DateUpdated = DateTime.Now;
            Set(model.GetType()).Add(model);
            SaveChanges();
            ModelCreated(model);
        }

        /// <summary>
        /// Updates a model in the database.
        /// </summary>
        /// <param name="model"></param>
        public virtual void UpdateModel(IModel model)
        {
            model.DateUpdated = DateTime.Now;
            Entry(model).State = System.Data.EntityState.Modified;
            SaveChanges();
            ModelUpdated(model);
        }

        /// <summary>
        /// Removes a model from the database.
        /// </summary>
        /// <param name="model"></param>
        public virtual void RemoveModel(IModel model)
        {
            Set(model.GetType()).Remove(model);
            SaveChanges();
            ModelRemoved(model);
        }
    }

    
}