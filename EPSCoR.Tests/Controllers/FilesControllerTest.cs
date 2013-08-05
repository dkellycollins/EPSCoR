using System;
using EPSCoR.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPSCoR.Tests.Controllers
{
    [TestClass]
    public class FilesControllerTest
    {
        private FilesController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _controller = new FilesController(null, null, null);
        }

        [TestMethod]
        public void UploadFileChunk()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void UploadFile()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void CheckFileThatHasNotBeenUploaded()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void CheckFileThatHasBeenPartiallyUploaded()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void CheckFileThatHasBeenUploaded()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void CompleteUploadOnExistingFile()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void CompleteUploadOnNonExistingFile()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void DownloadExistingFile()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void DownloadNonExistingFile()
        {
            Assert.Inconclusive();
        }
    }
}
