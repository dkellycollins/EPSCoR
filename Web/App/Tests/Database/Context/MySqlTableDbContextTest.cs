using System;
using EPSCoR.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPSCoR.Tests.Database.Context
{
    [TestClass]
    public class MySqlTableDbContextTest
    {
        [TestInitialize]
        public void Initailize()
        {
        }

        [TestMethod]
        public void SelectAllSuccessTest()
        {
            //Read in a full table.

            Assert.Inconclusive();
        }

        [TestMethod]
        public void SelectAllFailTest()
        {
            //Fail to select all from from a table that does not exist. Should return null.

            Assert.Inconclusive();
        }

        [TestMethod]
        public void SelectAllWithLimitsTest()
        {
            //Select all but limit the results.

            Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSqlException))]
        public void SelectAllInvalidSqlTest()
        {
            //Attempt to hack the db here. Should fail with an exception.

            Assert.Inconclusive();
        }

        [TestMethod]
        public void CountSuccessTest()
        {
            //Successfully get the number of row in a table.

            Assert.Inconclusive();
        }

        [TestMethod]
        public void CountFailTest()
        {
            //Return -1 for a table that does not exist.

            Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSqlException))]
        public void CountInvalidSqlTest()
        {
            //Attempt to hack the db here.

            Assert.Inconclusive();
        }

        [TestMethod]
        public void DropTableSuccessTest()
        {
            //Successfull drop a table.

            Assert.Inconclusive();
        }

        [TestMethod]
        public void DropTableFailTest()
        {
            //Try to drop a tabel that does not exist.

            Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSqlException))]
        public void DropTableInvalidSqlTest()
        {
            //Attempt to hack the db here.

            Assert.Inconclusive();
        }

        [TestMethod]
        public void AddTableFromFileSuccessTest()
        {
            //Successfully add a table from a file to the db.

            Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSqlException))]
        public void AddTableFromFieInvalidSqlTest()
        { 
            //Attampt to hack the db here.

            Assert.Inconclusive();
        }

        [TestMethod]
        public void PopuateTableFromFileTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSqlException))]
        public void PopulateTableFromFileInvalidSqlTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void SaveTableToFileTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSqlException))]
        public void SaveTableToFileInvalidSqlTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void SumTablesTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSqlException))]
        public void SumTablesInvalidSqlTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void AvgTablesTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidSqlException))]
        public void AvgTablesInvalidSqlTest()
        {
            Assert.Inconclusive();
        }
    }
}
