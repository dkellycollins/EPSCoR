using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using EPSCoR.Database.Models;

namespace EPSCoR.Repositories
{
    public class BasicTableRepo : ITableRepository, IDatabaseCalc
    {
        DefaultContext _context;

        public BasicTableRepo()
        {
            _context = DefaultContext.GetInstance();
        }

        #region IRawRepository Members

        public void Create(DataTable table)
        {
            throw new NotImplementedException();
        }

        public DataTable Read(string tableName)
        {
            return _context.Commands.SelectAllFrom(tableName);
        }

        public void Update(DataTable table)
        {
            throw new NotImplementedException();
        }

        public void Drop(string tableName)
        {
            TableIndex tableIndex = _context.Tables.Where((t) => t.Name == tableName).FirstOrDefault();
            if (tableIndex != null)
            {
                _context.Commands.DropTable(tableName);
                _context.Tables.Remove(tableIndex);
                _context.SaveChanges();
            }
        }

        public void Dispose()
        {
            DefaultContext.Release();
        }

        #endregion IRawRepository Members

        #region IDatabaseCalc Members

        public void SumTables(string attTable, string usTable)
        {
            _context.Commands.SumTables(attTable, usTable);
        }

        public void AvgTables(string attTable, string usTalbe)
        {
            throw new NotImplementedException();
        }

        #endregion IDatabaseCalc Members
    }
}