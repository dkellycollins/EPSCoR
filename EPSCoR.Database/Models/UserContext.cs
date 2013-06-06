using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPSCoR.Database.DbCmds;
using MySql.Data.MySqlClient;

namespace EPSCoR.Database.Models
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
                + "database=cybercomm-" + username + ";"
                + "DefaultCommandTimeout=0";
            DbConnection conn = new MySqlConnection(connection);
            
            UserContext context = new UserContext(conn);
            context.Commands = new MySqlCmd(context);

            return context;
        }

        public string DatabaseName 
        {
            get
            {
                return this.Database.Connection.Database;
            }
        }
        public DbCmd Commands { get; private set; }

        private UserContext(DbConnection conn)
            : base(conn, false)
        {
        }
    }
}
