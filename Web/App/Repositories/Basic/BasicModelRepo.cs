using System.Linq;
using EPSCoR.Web.Database;
using EPSCoR.Web.Database.Context;
using EPSCoR.Web.Database.Models;

namespace EPSCoR.Web.App.Repositories.Basic
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
            _context = DbContextFactory.GetModelDbContext();
        }

        public BasicModelRepo(ModelDbContext context)
        {
            _context = context;
        }

        public T Get(int entityID)
        {
            return _context.GetModel<T>(entityID);
        }

        public IQueryable<T> GetAll()
        {
            return _context.GetAllModels<T>();
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