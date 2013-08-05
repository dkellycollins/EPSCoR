using System;
using System.Web.Mvc;
using EPSCoR.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPSCoR.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        private AccountController _controller;

        [TestInitialize]
        public void Initalize()
        {
            _controller = new AccountController(null);
        }

        [TestMethod]
        public void GetNotAuthorized()
        {
            ActionResult result = _controller.NotAuthorized();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetCookiesRequired()
        {
            ActionResult result = _controller.CookiesRequired();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void LoginNewUser()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void LoginExistingUser()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void LogoffUser()
        {
            Assert.Inconclusive();
        }
    }
}
