using System.Data.Common;
using System.Data.Entity;
using EPSCoR.Database.DbProcedure;
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
                + "password=root;"
                + "Persist Security Info=True;"
                + "database=cybercomm_" + username + ";"
                + "DefaultCommandTimeout=0";
            DbConnection conn = new MySqlConnection(connection);
            
            UserContext context = new UserContext(conn);
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
        public DbProcedures Procedures { get; private set; }

        private UserContext(DbConnection conn)
            : base(conn, false)
        {
        }
    }
}
