using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using EPSCoR.Database.Models;

namespace EPSCoR.Repositories
{
    public class BasicRawRepo : IRawRepository
    {
        DefaultContext _context;

        public BasicRawRepo()
        {
            _context = DefaultContext.GetInstance();
        }

        public DataTable Get(string tableName)
        {
            return _context.GetTable(tableName);
        }

        public void Drop(string tableName)
        {
            _context.DropTable(tableName);
        }

        public void Dispose()
        {
            DefaultContext.Release();
        }
    }
}