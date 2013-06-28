using System.Data.Common;
using System.Data.Entity;
using System.IO;
using EPSCoR.Database.DbProcedure;
using EPSCoR.Database.Services;
using MySql.Data.MySqlClient;

namespace EPSCoR.Database
{
    public class UserContext : DbContext
    {
        public static UserContext GetContextForUser(string username)
        {
            string connection =
                "server=localhost;"
                + "User Id=root;"
                + "password=KsUwI1dC4tS!;"
                + "Persist Security Info=True;"
                + "database=cybercomm_" + username + ";"
                + "DefaultCommandTimeout=0";
            DbConnection conn = new MySqlConnection(connection);
            
            UserContext context = new UserContext(conn);
            context.User = username;
            context.Procedures = new MySqlProcedures(context);

            return context;
        }

        public string DatabaseName 
        {
            get
            {
                return this.Database.Connection.Database;
            }
        }
        public string User { get; private set; }
        public DbProcedures Procedures { get; private set; }

        private UserContext(DbConnection conn)
            : base(conn, false)
        {
        }

        public void SaveTableToFile(string table)
        {
            string filePath = Path.Combine(DirectoryManager.ConversionDir, User, table + ".csv");
            Procedures.SaveTableToFile(table, filePath);
        }
    }
}
