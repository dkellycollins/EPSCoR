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
    /// <summary>
    /// Since the user will be forced to login at just about any point in our site
    /// the AccountController.Login is never reached. 
    /// Using this attribute instead of just Authorize will ensure that the user is 
    /// added no matter what page they access first.
    /// </summary>
    public class AddUserWhenAuthorizedAttribute : AuthorizeAttribute
    {
        private IRepositoryFactory _repoFactory;

        public AddUserWhenAuthorizedAttribute()
        {
            _repoFactory = new RepositoryFactory();
        }

        /// <summary>
        /// Check to see if this is a new user. If so then add them to our database.
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (!string.IsNullOrEmpty(WebSecurity.CurrentUserName))
            {
                using (IModelRepository<UserProfile> repo = _repoFactory.GetModelRepository<UserProfile>())
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