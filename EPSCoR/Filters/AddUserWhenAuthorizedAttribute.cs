using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Factory;
using WebMatrix.WebData;

namespace EPSCoR.Filters
{
    public class AddUserWhenAuthorizedAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

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