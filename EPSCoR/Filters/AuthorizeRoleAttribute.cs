using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Basic;
using WebMatrix.WebData;

namespace EPSCoR.Filters
{
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        private UserProfile _currentUser = null;
        protected UserProfile CurrentUser
        {
            get
            {
                if (_currentUser == null || _currentUser.UserName != WebSecurity.CurrentUserName)
                {
                    _currentUser = _userRepo.GetAll().Where((x) => x.UserName == WebSecurity.CurrentUserName).FirstOrDefault();
                }
                return _currentUser;
            }
        }

        private IModelRepository<UserProfile> _userRepo;

        public AuthorizeRoleAttribute()
            : base()
        {
            _userRepo = new BasicModelRepo<UserProfile>();
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (CurrentUser != null)
                return this.Roles.Contains(CurrentUser.Role);
            return false;
        }
    }
}