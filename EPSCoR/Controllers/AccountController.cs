using System.Linq;
using System.Web.Mvc;
using DotNetCasClient;
using EPSCoR.Database.Models;
using EPSCoR.Filters;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Factory;
using WebMatrix.WebData;

namespace EPSCoR.Controllers
{
    /// <summary>
    /// Provides webpages and functions for loging a user in and out.
    /// </summary>
    [Authorize]
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
