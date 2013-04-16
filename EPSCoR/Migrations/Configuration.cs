namespace EPSCoR.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web.Security;
    using EPSCoR.Models;
    using WebMatrix.WebData;
    using EPSCoR.Filters;

    internal sealed class Configuration : DbMigrationsConfiguration<EPSCoR.Models.DefaultContext>
    {
        public Configuration()
        {
#if DEBUG
            AutomaticMigrationsEnabled = true;
#else
            AutomaticMigrationsEnabled = false;
#endif
        }

        protected override void Seed(DefaultContext context)
        {
#if DEBUG
            context.Database.Delete();
#endif
            context.Database.CreateIfNotExists();

            Assembly assembly = Assembly.GetExecutingAssembly();
            seedAccounts(assembly.GetManifestResourceStream("EPSCoR.App_Data.Seed_Data.AccountSeed.txt"));
        }

        private void seedAccounts(Stream resourceStream)
        {
            //Initializes websecurity
            new InitializeSimpleMembershipAttribute.SimpleMembershipInitializer();

            StreamReader reader = new StreamReader(resourceStream);
            string[] roles = reader.ReadLine().Split(',');
            foreach (string role in roles)
            {
                string trimmedRole = role.Trim();
                if (!Roles.RoleExists(trimmedRole))
                    Roles.CreateRole(trimmedRole);
            }

            string buffer;
            while ((buffer = reader.ReadLine()) != null)
            {
                string[] account = buffer.Split(',');
                string userName = account[0].Trim();
                string password = account[1].Trim();
                if (!WebSecurity.UserExists(account[0]))
                    WebSecurity.CreateUserAndAccount(
                        userName,
                        password);
                for(int i = 2; i < account.Length; i++)
                    Roles.AddUserToRole(userName, account[i].Trim());
            }
        }
    }
}
