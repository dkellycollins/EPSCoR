using System.Linq;
using System.Web.Mvc;
using DotNetCasClient;
using EPSCoR.Web.App.Filters;
using EPSCoR.Web.App.Repositories;
using EPSCoR.Web.App.Repositories.Factory;
using EPSCoR.Web.Database.Models;
using WebMatrix.WebData;

namespace EPSCoR.Web.App.Controllers
{
    /// <summary>
    /// Provides webpages and functions for loging a user in and out.
    /// </summary>
    [AddUserWhenAuthorized]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        private IModelRepository<UserProfile> _userProfileRepo;

        public AccountController()
        {
            _userProfileRepo = RepositoryFactory.GetModelRepository<UserProfile>();
        }

        public AccountController(IModelRepository<UserProfile> userProfileRepo)
        {
            _userProfileRepo = userProfileRepo;
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
            {
                createProfile();
            }
            return redirectToLocal(returnUrl);
        }

        [HttpGet]
        public ActionResult LogOff()
        {
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Logs the user out of Cas. Then redirect the user to the main page of the app.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff(string returnUrl)
        {
            CasAuthentication.SingleSignOut();
            return redirectToLocal(returnUrl);
        }

        public ActionResult NotAuthorized()
        {
            return View();
        }

        public ActionResult CookiesRequired()
        {
            return View();
        }

        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        private void createProfile()
        {
            UserProfile profile = new UserProfile()
            {
                UserName = WebSecurity.CurrentUserName
            };
            _userProfileRepo.Create(profile);
        }

        /// <summary>
        /// If the url is a local url redirect to that. Otherwise redirect to the homepage.
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
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
