using System.Data.Entity;
using System.Linq;
using EPSCoR.Database.DbProcedure;
using EPSCoR.Database.Models;

namespace EPSCoR.Database
{
    public class DefaultContext : DbContext
    {
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

        public DefaultContext()
            : base("MySqlConnection")
        {
            Procedures = new MySqlProcedures(this);
        }

        public TableIndex GetTableIndex(string tableName, string UserName)
        {
            return Tables.Where(index => index.Name == tableName && index.UploadedByUser == UserName).FirstOrDefault();
        }
    }
}