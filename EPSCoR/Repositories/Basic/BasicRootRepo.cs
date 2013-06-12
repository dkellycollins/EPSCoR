using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPSCoR.Database;
using EPSCoR.Database.Models;

namespace EPSCoR.Repositories.Basic
{
    public class BasicRootRepo : IRootRepository
    {
        private DefaultContext _context;

        public BasicRootRepo()
        {
            _context = new DefaultContext();
        }

        public void CreateDatabase(string databaseName)
        {
            _context.Commands.CreateDatabase(databaseName);
        }

        public void DropDatabase(string databaseName)
        {
            //_context.Commands.DropDatabase(databaseName);
        }
    
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}