using EPSCoR.Database;
using EPSCoR.Database.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EPSCoR.Repositories.Async
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
            return ReadTaskAsync(tableName).Result;
        }

        public async Task<DataTable> ReadTaskAsync(string tableName)
        {
            return await Task.Run(() => _userContext.Procedures.SelectAllFrom(tableName));
        }

        public DataTable Read(string tableName, int lowerLimit, int upperLimit)
        {
            return ReadTaskAsync(tableName, lowerLimit, upperLimit).Result;
        }

        public async Task<DataTable> ReadTaskAsync(string tableName, int lowerLimit, int upperLimit)
        {
            int totalRows = 0;
            return await Task.Run(() => _userContext.Procedures.SelectAllFrom(tableName, lowerLimit, upperLimit, out totalRows));
        }

        public int Count(string tableName)
        {
            return CountTaskAsync(tableName).Result;
        }

        public async Task<int> CountTaskAsync(string tableName)
        {
            return await Task.Run(() => _userContext.Procedures.Count(tableName));
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

        public async void DropTaskAsync(string tableName)
        {
            await Task.Run(() =>
            {
                _userContext.Procedures.DropTable(tableName);

                TableIndex tableIndex = _defaultContext.Tables.Where((t) => t.Name == tableName).FirstOrDefault();
                if (tableIndex != null)
                {
                    _defaultContext.Tables.Remove(tableIndex);
                    _defaultContext.SaveChanges();
                }
            });
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
            return createCalcTableTaskAsync(attTable, usTable, CalcType.SUM).Result;
        }

        public CalcResult AvgTables(string attTable, string usTable)
        {
            return createCalcTableTaskAsync(attTable, usTable, CalcType.AVG).Result;
        }

        #endregion IDatabaseCalc Members

        private enum CalcType
        {
            SUM,
            AVG,
        }

        private async Task<CalcResult> createCalcTableTaskAsync(string attTable, string usTable, CalcType calc)
        {
            string calcTable = string.Format("{0}_{1}_SUM", attTable, usTable);
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

            return await Task.Run(() =>
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
                        throw new Exception("Unknown calc type.");
                }
            }).ContinueWith((task) =>
            {
                CalcResult result;
                if (task.IsFaulted)
                {
                    index.Status = "The server encountered an error while processing your request.";
                    result = CalcResult.Error;
                }
                else
                {
                    index.Status = "Table created.";
                    index.Processed = true;
                    result = CalcResult.Success;
                }

                index.DateUpdated = DateTime.Now;
                _defaultContext.Entry(index).State = EntityState.Modified;
                _defaultContext.SaveChanges();
                return result;
            });
        }
    }
}