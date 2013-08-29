using System.Configuration;
using System.Data;
using System.Data.Common;
using EPSCoR.Web.Database.Context;
using EPSCoR.Web.Database.Context.MySql;
using MySql.Data.MySqlClient;

namespace EPSCoR.Web.Database
{
    public interface IDbContextFactory
    {
        ModelDbContext GetModelDbContext();
        ModelDbContext GetModelDbContextForUser(string userName);
        TableDbContext GetTableDbContext();
        TableDbContext GetTableDbContextForUser(string userName);
    }

    public class DbContextFactory : IDbContextFactory
    {
        public ModelDbContext GetModelDbContext()
        {
            return new MySqlModelDbContext();
        }

        public ModelDbContext GetModelDbContextForUser(string userName)
        {
            MySqlConnection connection = new MySqlConnection(getUserConnectionString(userName));
            return new MySqlModelDbContext(connection);
        }

        public TableDbContext GetTableDbContext()
        {
            return new MySqlTableDbContext();
        }

        public TableDbContext GetTableDbContextForUser(string userName)
        {
            MySqlConnection connection = new MySqlConnection(getUserConnectionString(userName));
            return new MySqlTableDbContext(connection);
        }

        private string getUserConnectionString(string userName)
        {
            ConnectionStringSettings csSettings = ConfigurationManager.ConnectionStrings["MySqlConnection"];

            MySqlConnectionStringBuilder csBuilder = new MySqlConnectionStringBuilder(csSettings.ConnectionString);
            csBuilder.Database += "_" + userName;

            return csBuilder.GetConnectionString(true);
        }
    }
}