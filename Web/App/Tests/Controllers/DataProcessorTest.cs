using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPSCoR.Tests.Controllers
{
    [TestClass]
    public class DataProcessorTest
    {
        [TestMethod]
        public void NavigateToIndexNotLoggedIn()
        {
            //Should redirct to login page.
            Assert.Inconclusive();
        }

        [TestMethod]
        public void NavigateToIndexLoggedIn()
        {
            //Should load index page.
            //Also verify that a call to get the data is made.

            Assert.Inconclusive();
        }

        [TestMethod]
        public void UploadFileTest()
        {
            //Test the upload file workflow.

            Assert.Inconclusive();
        }

        [TestMethod]
        public void DeleteTableTest()
        {
            //Test deleting the table workflow.

            Assert.Inconclusive();
        }

        [TestMethod]
        public void DownloadTableTest()
        {
            //Test the download workflow.

            Assert.Inconclusive();
        }
    }
}
