using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using EPSCoR.Database;
using EPSCoR.Database.Models;

namespace EPSCoR.Repositories.Basic
{
    public class BasicTableRepo : ITableRepository, IDatabaseCalc
    {
        DefaultContext _defaultContext;
        UserContext _userContext;
        string currentUser;

        public BasicTableRepo(string userName)
        {
            _defaultContext = new DefaultContext();
            _userContext = UserContext.GetContextForUser(userName);
            currentUser = userName;
        }

        #region IRawRepository Members

        public void Create(DataTable table)
        {
            throw new NotImplementedException();
        }

        public DataTable Read(string tableName)
        {
            try
            {
                return _userContext.Procedures.SelectAllFrom(tableName);
            }
            catch
            {
                return null;
            }
        }

        public void Update(DataTable table)
        {
            throw new NotImplementedException();
        }

        public void Drop(string tableName)
        {
            _userContext.Procedures.DropTable(tableName);

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

        public CalcResult SumTables(string attTable, string usTable)
        {
            string calcTable = string.Format("{0}_{1}_SUM", attTable, usTable);
            TableIndex exisitingTable = _defaultContext.GetTableIndex(calcTable, currentUser);
            if (exisitingTable != null)
                return CalcResult.TableAlreadyExists;

            try
            {
                TableIndex index = new TableIndex()
                {
                    Name = calcTable,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Status = "Generating table.",
                    Type = TableTypes.CALC,
                    UploadedByUser = currentUser
                };
                _defaultContext.Tables.Add(index);
                _defaultContext.SaveChanges();

                _userContext.Procedures.SumTables(attTable, usTable, calcTable);

                index.Status = "Table created.";
                index.DateUpdated = DateTime.Now;
                _defaultContext.Entry(index).State = EntityState.Modified;
                _defaultContext.SaveChanges();

                return CalcResult.Success;
            }
            catch
            {
                return CalcResult.Error;
            }
        }

        public void SumTablesAsync(string attTable, string usTable)
        {
            string calcTable = string.Format("{0}_{1}_SUM", attTable, usTable);
            TableIndex index = new TableIndex()
            {
                Name = calcTable,
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Status = "Generating table.",
                Type = TableTypes.CALC,
                UploadedByUser = currentUser
            };
            _defaultContext.Tables.Add(index);
            _defaultContext.SaveChanges();

            Task.Factory.StartNew(() => _userContext.Procedures.SumTables(attTable, usTable, calcTable))
                .ContinueWith((task) =>
                {
                    index.Status = "Table created.";
                    index.DateUpdated = DateTime.Now;
                    _defaultContext.Entry(index).State = EntityState.Modified;
                    _defaultContext.SaveChanges();
                });
        }

        public CalcResult AvgTables(string attTable, string usTalbe)
        {
            throw new NotImplementedException();
        }

        #endregion IDatabaseCalc Members
    }
}