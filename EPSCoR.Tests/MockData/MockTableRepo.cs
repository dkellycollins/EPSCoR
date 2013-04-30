using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPSCoR.Models;
using EPSCoR.Repositories;

namespace EPSCoR.Tests.MockData
{
    public class MockTableRepo : IRepository<Table>
    {
        public Table Get(int entityID)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Table> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Create(Table itemToCreate)
        {
            throw new NotImplementedException();
        }

        public void Update(Table itemToUpdate)
        {
            throw new NotImplementedException();
        }

        public void Remove(int entityID)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
