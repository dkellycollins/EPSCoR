using System;
using EPSCoR.Database.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPSCoR.Tests.Database.Context
{
    [TestClass]
    public class MySqlModelDbContextTest
    {
        MySqlModelDbContext _context;

        [TestInitialize]
        public void Initialize()
        {
            
        }

        [TestMethod]
        public void GetModelSuccessTest()
        {
            //Sucessfully get a model from the db.

            Assert.Inconclusive();
        }

        public void GetModelFailTest()
        {
            //Fail to get amodel with an invalid id.

            Assert.Inconclusive();
        }

        public void GetAllModelsTest()
        {
            //Get all models from the db.

            Assert.Inconclusive();
        }

        public void CreateModelTest()
        {
            //Succesfully create a new model in the db. Should also call ModelCreated.
        }

        public void CreateDuplicateModelTest()
        {
            //Fail to create a model already in the db. Should not call ModelCreated.

            Assert.Inconclusive();
        }

        public void UpdateModelTest()
        {
            //Succefully update a model with new information. Should also call ModelUpdated.

            Assert.Inconclusive();
        }

        public void UpdateNonExistingModelTest()
        {
            //Fail to create update a model that does not exist yet. Should not call ModelCreated.

            Assert.Inconclusive();
        }

        public void RemoveModelTest()
        {
            //Successfully remove a model from the db. Should also call ModelRemoved.

            Assert.Inconclusive();
        }

        public void RemoveNonExistingModelTest()
        {
            //Fail to remove a model that does not exist. Should not call ModelRemoved.

            Assert.Inconclusive();
        }
    }
}
