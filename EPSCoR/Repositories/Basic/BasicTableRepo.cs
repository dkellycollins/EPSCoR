using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using EPSCoR.Database.Models;

namespace EPSCoR.Repositories.Basic
{
    public class BasicTableRepo : ITableRepository, IDatabaseCalc
    {
        DefaultContext _defaultContext;
        UserContext _userContext;

        public BasicTableRepo(string userName)
        {
            _defaultContext = new DefaultContext();
            _userContext = UserContext.GetContextForUser(userName);
        }

        #region IRawRepository Members

        public void Create(DataTable table)
        {
            throw new NotImplementedException();
        }

        public DataTable Read(string tableName)
        {
            return _userContext.Commands.SelectAllFrom(tableName);
        }

        public void Update(DataTable table)
        {
            throw new NotImplementedException();
        }

        public void Drop(string tableName)
        {
            _userContext.Commands.DropTable(tableName);

            TableIndex tableIndex = _defaultContext.Tables.Where((t) => t.Name == tableName).FirstOrDefault();
            if (tableIndex != null)
            {
                _defaultContext.Tables.Remove(tableIndex);
                _defaultContext.SaveChanges();
            }
        }

        public void Dispose()
        {
            _defaultContext.Dispose();
            _userContext.Dispose();
        }

        #endregion IRawRepository Members

        #region IDatabaseCalc Members

        public void SumTables(string attTable, string usTable)
        {
            _userContext.Commands.SumTables(attTable, usTable);
        }

        public void AvgTables(string attTable, string usTalbe)
        {
            throw new NotImplementedException();
        }

        #endregion IDatabaseCalc Members
    }
}