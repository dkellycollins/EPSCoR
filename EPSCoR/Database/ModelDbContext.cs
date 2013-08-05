using System;
using System.Data.Entity;
using System.Linq;
using EPSCoR.Database.DbProcedure;
using EPSCoR.Database.Models;

namespace EPSCoR.Database
{
    /// <summary>
    /// Provides access the default database.
    /// </summary>
    public class ModelDbContext : DbContext
    {
        public delegate void ModelEventHandler(IModel model);

        public static event ModelEventHandler ModelCreated = delegate { };
        public static event ModelEventHandler ModelUpdated = delegate { };
        public static event ModelEventHandler ModelRemoved = delegate { };

        /*
        private static DefaultContext _instance = null;
        private static int referenceCount = 0;

        public static DefaultContext GetInstance()
        {
            Interlocked.Increment(ref referenceCount);
            if (referenceCount == 1)
            {
                _instance = new DefaultContext();
            }
            return _instance;
        }

        public static void Release()
        {
            Interlocked.Decrement(ref referenceCount);
            if (referenceCount == 0)
            {
                _instance.Dispose();
                _instance = null;
            }
        }
         */

        public DbSet<TableIndex> Tables { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbProcedures Procedures { get; private set; }

        public ModelDbContext()
            : base("MySqlConnection")
        {
            Procedures = new MySqlProcedures(this);
        }

        public virtual void CreateModel(IModel model)
        {
            model.DateCreated = DateTime.Now;
            model.DateUpdated = DateTime.Now;

            if (model is TableIndex)
            {
                Tables.Add((TableIndex)model);
            }
            else if (model is UserProfile)
            {
                UserProfiles.Add((UserProfile)model);
            }
            else
            {
                throw new Exception("Unsupported model type");
            }

            SaveChanges();
            ModelCreated(model);
        }

        public virtual void UpdateModel(IModel model)
        {
            model.DateUpdated = DateTime.Now;

            if (model is TableIndex)
            {
                Entry<TableIndex>((TableIndex)model).State = System.Data.EntityState.Modified;
            }
            else if (model is UserProfile)
            {
                Entry<UserProfile>((UserProfile)model).State = System.Data.EntityState.Modified;
            }
            else
            {
                throw new Exception("Unsupported model type");
            }

            SaveChanges();
            ModelUpdated(model);
        }

        public virtual void RemoveModel(IModel model)
        {
            if (model is TableIndex)
            {
                Tables.Remove((TableIndex)model);
            }
            else if (model is UserProfile)
            {
                UserProfiles.Remove((UserProfile)model);
            }
            else
            {
                throw new Exception("Unsupported model type.");
            }

            SaveChanges();
            ModelRemoved(model);
        }

        public TableIndex GetTableIndex(string tableName, string UserName)
        {
            return Tables.Where(index => index.Name == tableName && index.UploadedByUser == UserName).FirstOrDefault();
        }
    }
}