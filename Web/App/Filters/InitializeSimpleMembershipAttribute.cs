using System;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using EPSCoR.Web.Database;
using EPSCoR.Web.Database.Context;
using WebMatrix.WebData;

namespace EPSCoR.Web.App.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ensure ASP.NET Simple Membership is initialized only once per app start
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        //Marked as internal so the seed method can get access to this.
        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                System.Data.Entity.Database.SetInitializer<ModelDbContext>(null);

                var context = DbContextFactory.GetModelDbContext();
                try
                {
                    if (!context.Database.Exists())
                    {
                        // Create the SimpleMembership database without Entity Framework migration schema
                        ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                    }

                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }
    }
}
