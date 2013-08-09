using System.Data.Entity;
using System.Linq;
using EPSCoR.Database.Models;
using MySql.Data.MySqlClient;

namespace EPSCoR.Database.Context
{
    /// <summary>
    /// 
    /// </summary>
    public class MySqlModelDbContext : ModelDbContext
    {
        private DbSet<TableIndex> Tables { get; set; }
        private DbSet<UserProfile> UserProfiles { get; set; }
        private DbSet<UserConnection> UserConnections { get; set; }

        public MySqlModelDbContext()
            : base("MySqlConnection")
        { }

        public MySqlModelDbContext(MySqlConnection connection)
            : base(connection)
        { }

        public TableIndex GetTableIndex(string tableName, string UserName)
        {
            return GetAllModels<TableIndex>().Where(index => index.Name == tableName && index.UploadedByUser == UserName).FirstOrDefault();
        }
    }
}