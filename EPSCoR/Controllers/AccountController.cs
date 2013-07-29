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
using EPSCoR.Repositories.Basic;
using EPSCoR.Repositories.Factory;

namespace EPSCoR.Controllers
{
    /// <summary>
    /// Provides webpages and functions for loging a user in and out.
    /// </summary>
    [EPSCoR.Filters.Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        private IModelRepository<UserProfile> _userProfileRepo;

        public AccountController()
        {
            _userProfileRepo = RepositoryFactory.GetModelRepository<UserProfile>();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            _userProfileRepo.Dispose();

            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        /// This will redirect the user to the CAS login page. Once the user is authenticated, adds the user to our database if needed. 
        /// </summary>
        /// <param name="returnUrl">Url to redirect the user to.</param>
        /// <returns></returns>
        public ActionResult Login(string returnUrl)
        {
            //This is where we should check to see if the user has an account.
            UserProfile profile = _userProfileRepo.GetAll().Where((x) => x.UserName == WebSecurity.CurrentUserName).FirstOrDefault();

            if (profile == null)
                createProfile();
            return redirectToLocal(returnUrl);
        }

        /// <summary>
        /// Logs the user out of Cas. Then redirect the user to the main page of the app.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LogOff()
        {
            CasAuthentication.SingleSignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult NotAuthorized()
        {
            return View();
        }

        public ActionResult CookiesRequired()
        {
            return View();
        }

        //Adds a new user to the database.
        private void createProfile()
        {
            UserProfile profile = new UserProfile()
            {
                UserName = WebSecurity.CurrentUserName
            };
            _userProfileRepo.Create(profile);
        }

        //If the url is a local url redirect to that. Otherwise redirect to the homepage.
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
