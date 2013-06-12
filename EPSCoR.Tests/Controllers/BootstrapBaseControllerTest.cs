using System;
using BootstrapSupport;
using EPSCoR.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPSCoR.Tests.Controllers
{
    [TestClass]
    public class BootstrapBaseControllerTest
    {
        private BootstrapBaseController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _controller = new BootstrapBaseController();
        }

        [TestMethod]
        public void DisplayAttentionTest()
        {
            string expected = "Testing";

            _controller.DisplayAttention(expected);
            var result = _controller.TempData[Alerts.ATTENTION];

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DisplaySuccessTest()
        {
            string expected = "Testing";

            _controller.DisplaySuccess(expected);
            var result = _controller.TempData[Alerts.SUCCESS];

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DisplayInformationTest()
        {
            string expected = "Testing";

            _controller.DisplayInformation(expected);
            var result = _controller.TempData[Alerts.INFORMATION];

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DisplayErrorTest()
        {
            string expected = "Testing";

            _controller.DisplayError(expected);
            var result = _controller.TempData[Alerts.ERROR];

            Assert.AreEqual(expected, result);
        }
    }
}
