using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using EPSCoR.Database.Models;
using EPSCoR.Filters;
using EPSCoR.Repositories;
using DotNetCasClient;

namespace EPSCoR.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        private IModelRepository<UserProfile> _userProfileRepo;

        public AccountController()
        {
            _userProfileRepo = new BasicModelRepo<UserProfile>();
        }

        //
        // GET: /Account/Login
        public ActionResult Login(string returnUrl)
        {
            //This is where we should check to see if the user has an account.
            UserProfile profile = _userProfileRepo.GetAll().Where((x) => x.UserName == WebSecurity.CurrentUserName).FirstOrDefault();

            if (profile == null)
                createProfile();
            return redirectToLocal(returnUrl);
        }

        private void createProfile()
        {
            UserProfile profile = new UserProfile()
            {
                UserName = WebSecurity.CurrentUserName
            };
            _userProfileRepo.Create(profile);
        }

        //
        // POST: /Account/LogOff
        public ActionResult LogOff()
        {
            CasAuthentication.SingleSignOut();
            return RedirectToAction("Index", "Home");
        }

        private ActionResult redirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
