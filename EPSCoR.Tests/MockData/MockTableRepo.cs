using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPSCoR.Models;
using EPSCoR.Repositories;

namespace EPSCoR.Tests.MockData
{
    public class MockTableRepo : IRepository<TableIndex>
    {
        public TableIndex Get(int entityID)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TableIndex> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Create(TableIndex itemToCreate)
        {
            throw new NotImplementedException();
        }

        public void Update(TableIndex itemToUpdate)
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
