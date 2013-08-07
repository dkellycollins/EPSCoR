using EPSCoR.Database.Context;
using MySql.Data.MySqlClient;

namespace EPSCoR.Database
{
    public class DbContextFactory
    {
        private const string USER_CONNECTION_STRING = "server=localhost;"
            + "User Id=root;"
            + "Persist Security Info=True;"
            + "database=cybercomm_{0};"
#if DEBUG
            + "password=root;"
#else
            + "password=KsUwI1dC4tS!"
#endif
            + "DefaultCommandTimeout=0;";

        public static ModelDbContext GetModelDbContext()
        {
            return new MySqlModelDbContext();
        }

        public static ModelDbContext GetModelDbContextForUser(string userName)
        {
            MySqlConnection connection = new MySqlConnection(string.Format(USER_CONNECTION_STRING, userName));
            return new MySqlModelDbContext(connection);
        }

        public static TableDbContext GetTableDbContext()
        {
            return new MySqlTableDbContext();
        }

        public static TableDbContext GetTableDbContextForUser(string userName)
        {
            MySqlConnection connection = new MySqlConnection(string.Format(USER_CONNECTION_STRING, userName));
            return new MySqlTableDbContext(connection);
        }
    }
}