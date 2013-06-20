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
            return createCalcTable(attTable, usTable, CalcType.SUM);
        }

        public CalcResult AvgTables(string attTable, string usTable)
        {
            return createCalcTable(attTable, usTable, CalcType.AVG);
        }

        #endregion IDatabaseCalc Members

        private enum CalcType
        {
            SUM,
            AVG,
        }

        private CalcResult createCalcTable(string attTable, string usTable, CalcType calc)
        {
            string calcTable = string.Format("{0}_{1}_{2}", attTable, usTable, calc.ToString());
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
                    UploadedByUser = currentUser,
                    Processed = false
                };
                _defaultContext.Tables.Add(index);
                _defaultContext.SaveChanges();

                switch (calc)
                {
                    case CalcType.SUM:
                        _userContext.Procedures.SumTables(attTable, usTable, calcTable);
                        break;
                    case CalcType.AVG:
                        _userContext.Procedures.AvgTables(attTable, usTable, calcTable);
                        break;
                    default:
                        throw new Exception("Unknown calctype");
                }

                index.Status = "Table created.";
                index.Processed = true;
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

        private CalcResult createCalcTableAsync(string attTable, string usTable, CalcType calc)
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

            Task.Factory.StartNew(() =>
            {
                switch (calc)
                {
                    case CalcType.SUM:
                        _userContext.Procedures.SumTables(attTable, usTable, calcTable);
                        break;
                    case CalcType.AVG:
                        _userContext.Procedures.AvgTables(attTable, usTable, calcTable);
                        break;
                    default:
                        throw new Exception("Unknown calctype");
                }
            })
                .ContinueWith((task) =>
                {
                    index.Status = "Table created.";
                    index.DateUpdated = DateTime.Now;
                    _defaultContext.Entry(index).State = EntityState.Modified;
                    _defaultContext.SaveChanges();
                });

            return CalcResult.SubmittedForProcessing;
        }
    }
}