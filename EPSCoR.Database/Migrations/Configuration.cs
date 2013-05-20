namespace EPSCoR.Database.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web.Security;
    using EPSCoR.Database.Models;
    using WebMatrix.WebData;

    internal sealed class Configuration : DbMigrationsConfiguration<EPSCoR.Database.Models.DefaultContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(DefaultContext context)
        {
#if DEBUG
            context.Database.Delete();
#endif
            context.Database.CreateIfNotExists();

            //Get the assembly that contains this class.
            Assembly assembly = Assembly.GetAssembly(typeof(Configuration));
            //seedAccounts(assembly.GetManifestResourceStream("EPSCoR.Database.App_Data.Seed_Data.AccountSeed.txt"));
            seedAccounts();
        }

        private void seedAccounts(Stream resourceStream)
        {
            //Initializes websecurity
            new SimpleMembershipInitializer();

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

        private void seedAccounts()
        {
            new SimpleMembershipInitializer();

            string[] roles = new string[]
            {
                "admin",
                "default"
            };

            foreach (string role in roles)
            {
                if (!Roles.RoleExists(role))
                    Roles.CreateRole(role);
            }

            List<Tuple<string, string, string>> accounts = new List<Tuple<string,string,string>>()
            {
                new Tuple<string, string, string>("ram", "Ircees60Joib_@", "admin")
            };

            foreach (Tuple<string, string, string> account in accounts)
            {
                if (!WebSecurity.UserExists(account.Item1))
                {
                    WebSecurity.CreateUserAndAccount(
                        account.Item1,
                        account.Item2);
                    Roles.AddUserToRole(account.Item1, account.Item3);
                }
            }

        }

        internal class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                System.Data.Entity.Database.SetInitializer<DefaultContext>(null);

                try
                {
                    using (var context = DefaultContext.GetInstance())
                    {
                        if (!context.Database.Exists())
                        {
                            // Create the SimpleMembership database without Entity Framework migration schema
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }

                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
                finally
                {
                    DefaultContext.Release();
                }
            }
        }
    }
}
