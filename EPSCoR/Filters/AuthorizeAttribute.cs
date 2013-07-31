using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Factory;
using WebMatrix.WebData;

namespace EPSCoR.Filters
{
    /*
    public class AuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            //This is where we should check to see if the user has an account.
            using (IModelRepository<UserProfile> repo = RepositoryFactory.GetModelRepository<UserProfile>())
            {
                UserProfile profile = repo.GetAll().Where((x) => x.UserName == WebSecurity.CurrentUserName).FirstOrDefault();

                if (profile == null)
                {
                    profile = new UserProfile()
                    {
                        UserName = WebSecurity.CurrentUserName
                    };
                    repo.Create(profile);
                }
            }
        }
    }
    */
}