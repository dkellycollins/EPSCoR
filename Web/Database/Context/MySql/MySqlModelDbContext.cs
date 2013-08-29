using System.Data.Entity;
using System.Linq;
using EPSCoR.Web.Database.Models;
using MySql.Data.MySqlClient;

namespace EPSCoR.Web.Database.Context.MySql
{
    /// <summary>
    /// 
    /// </summary>
    public class MySqlModelDbContext : ModelDbContext
    {
        private DbSet<TableIndex> Tables { get; set; }
        private DbSet<UserProfile> UserProfiles { get; set; }
        private DbSet<UserConnection> UserConnections { get; set; }
        private DbSet<LogEntry> LogEntries { get; set; }
        private DbSet<DbEvent> Events { get; set; }

        public MySqlModelDbContext()
            : base("MySqlConnection")
        { }

        public MySqlModelDbContext(MySqlConnection connection)
            : base(connection)
        {
        }

        public TableIndex GetTableIndex(string tableName, string UserName)
        {
            return GetAllModels<TableIndex>().Where(index => index.Name == tableName && index.UploadedByUser == UserName).FirstOrDefault();
        }
    }
}