using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using EPSCoR.Database.DbCmds;

namespace EPSCoR.Database.Models
{
    public class DefaultContext : DbContext
    {
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

        public DbSet<TableIndex> Tables { get; set; }
        public DbSet<TablePairIndex> TablePairs { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        public DefaultContext()
            : base("MySqlConnection")
        {
        }

        public IEnumerable<AttributeData> GetAllFromAttributeTable(string attTable)
        {
            return this.Database.SqlQuery<AttributeData>("SELECT * FROM " + attTable);
        }

        public IEnumerable<UpstreamData> GetAllFromUpstreamTable(string usTable)
        {
            return this.Database.SqlQuery<UpstreamData>("SELECT * FROM " + usTable);
        }

        public void DeleteTable(string table)
        {
            TableIndex tableIndex = Tables.Where((t) => t.Name == table).FirstOrDefault();
            if (tableIndex != null)
            {
                this.Database.ExecuteSqlCommand("DROP TABLE " + table);
                Tables.Remove(tableIndex);
                SaveChanges();
            }
        }
    }
}