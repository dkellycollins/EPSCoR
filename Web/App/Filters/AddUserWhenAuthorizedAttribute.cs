using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPSCoR.Web.App.Repositories;
using EPSCoR.Web.App.Repositories.Factory;
using EPSCoR.Web.Database.Models;
using WebMatrix.WebData;

namespace EPSCoR.Web.App.Filters
{
    public class AddUserWhenAuthorizedAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (!string.IsNullOrEmpty(WebSecurity.CurrentUserName))
            {
                using (IModelRepository<UserProfile> repo = RepositoryFactory.GetModelRepository<UserProfile>())
                {
                    UserProfile user = repo.GetAll().Where((u) => u.UserName == WebSecurity.CurrentUserName).FirstOrDefault();

                    if (user == null)
                    {
                        repo.Create(new UserProfile()
                        {
                            UserName = WebSecurity.CurrentUserName
                        });
                    }
                }
            }
        }
    }
}