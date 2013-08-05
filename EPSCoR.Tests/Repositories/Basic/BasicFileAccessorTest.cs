using System;
using System.Collections.Generic;
using System.IO;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Basic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EPSCoR.Tests.Repositories.Basic
{
    [TestClass]
    public class BasicFileAccessorTest
    {
        private BasicFileAccessor _fileAccessor;
        private FileDirectory _mainDir;
        private FileDirectory _altDir;
        private string _fileName;

        [TestInitialize]
        public void Initialize()
        {
            string userName = "TestUser";
            Mock<IDirectoryResolver> mockDirectoryResolver = new Mock<IDirectoryResolver>();
            _mainDir = FileDirectory.Upload;
            _altDir = FileDirectory.Temp;
            string mainDirPath = Path.Combine(Environment.CurrentDirectory, "Upload", userName);
            string altDirPath = Path.Combine(Environment.CurrentDirectory, "Temp", userName);

            mockDirectoryResolver.Setup(resolver => resolver.GetUserDirectory(_mainDir, userName))
                .Returns(mainDirPath);
            mockDirectoryResolver.Setup(resolver => resolver.GetUserDirectory(_altDir, userName))
                .Returns(altDirPath);
            _fileName = "MDC_B_ATT";

            _fileAccessor = new BasicFileAccessor(userName, mockDirectoryResolver.Object);
        }

        [TestMethod]
        public void SaveFullFileTest()
        {
            string fileName = null;
            FileStream testFileStream = new FileStream(fileName, FileMode.Open);

            _fileAccessor.SaveFiles(_mainDir, new FileStreamWrapper()
            {
                FileName = fileName,
                FileSize = (int)testFileStream.Length,
                InputStream = testFileStream,
                SeekPos = 0
            });

            Assert.Inconclusive();
        }

        public void SavePartialFileTest()
        {
            string fileName = null;
            FileStream testFileStream = new FileStream(fileName, FileMode.Open);

            _fileAccessor.SaveFiles(_mainDir, new FileStreamWrapper()
            {
                FileName = fileName,
                FileSize = (int)testFileStream.Length,
                InputStream = testFileStream,
                SeekPos = 0
            });

            Assert.Inconclusive();
        }

        [TestMethod]
        public void OpenExistingFileTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void OpenNonExistingFileTest()
        {
            FileStream result = _fileAccessor.OpenFile(_mainDir, "NonExistingFile");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetFilesTest()
        {
            List<string> results = new List<string>(_fileAccessor.GetFiles(_mainDir));

            Assert.AreEqual(results.Count, 1);
            Assert.AreEqual(results[0], _fileName);
        }

        [TestMethod]
        public void DeleteFilesTest()
        {
            _fileAccessor.DeleteFiles(_mainDir, _fileName);

            Assert.Inconclusive();
        }

        [TestMethod]
        public void FileDoesExistTest()
        {
            bool result = _fileAccessor.FileExist(_mainDir, "NonExistingFile");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void FileDoesNotExistTest()
        {
            bool result = _fileAccessor.FileExist(_mainDir, "NonExistingFile");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void FileInfoTest()
        {
            FileInfo result = _fileAccessor.GetFileInfo(_mainDir, _fileName);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Exists);
        }

        [TestMethod]
        public void MoveExistingFileTest()
        {
            _fileAccessor.MoveFile(_mainDir, _altDir, _fileName);

            Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void MoveNonExistingFileTest()
        {
            _fileAccessor.MoveFile(_mainDir, _altDir, "NonExistingFile");

            Assert.Fail("MoveFile should have failed");
        }
    }
}
