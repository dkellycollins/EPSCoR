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
    public class AsyncTableRepo : IAsyncTableRepository, IAsyncDatabaseCalc
    {
        private DefaultContext _defaultContext;
        
        UserContext _userContext;
        string currentUser;

        public AsyncTableRepo(string userName)
        {
            _defaultContext = new DefaultContext();
            _userContext = UserContext.GetContextForUser(userName);
            currentUser = userName;
        }

        #region IAsyncTableRepository Members

        public async Task CreateAsync(DataTable table)
        {
            throw new NotImplementedException();
        }

        public async Task<DataTable> ReadAsync(string tableName)
        {
            return await Task.Run(() => _userContext.Procedures.SelectAllFrom(tableName));
        }

        public async Task<DataTable> ReadAsync(string tableName, int lowerLimit, int upperLimit)
        {
            int totalRows = 0;
            return await Task.Run(() => _userContext.Procedures.SelectAllFrom(tableName, lowerLimit, upperLimit, out totalRows));
        }

        public async Task<int> CountAsync(string tableName)
        {
            return await Task.Run(() => _userContext.Procedures.Count(tableName));
        }

        public async Task UpdateAsync(DataTable table)
        {
            throw new NotImplementedException();
        }

        public async Task DropAsync(string tableName)
        {
            await Task.Run(() =>
            {
                _userContext.Procedures.DropTable(tableName);

                TableIndex tableIndex = _defaultContext.Tables.Where((t) => t.Name == tableName).FirstOrDefault();
                if (tableIndex != null)
                {
                    _defaultContext.RemoveModel(tableIndex);
                }
            });
        }

        #endregion IAsyncTableRepository

        #region IAysncDatabaseCalc Members

        public Task<CalcResult> JoinTablesAsync(string attTable, string usTable, string calcType)
        {
            return createCalcTableTaskAsync(attTable, usTable, calcType);
        }

        public Task<CalcResult> SumTablesAsync(string attTable, string usTable)
        {
            return createCalcTableTaskAsync(attTable, usTable, CalcType.Sum);
        }

        public Task<CalcResult> AvgTablesAsync(string attTable, string usTable)
        {
            return createCalcTableTaskAsync(attTable, usTable, CalcType.Avg);
        }

        #endregion IAsyncDatabaseCalc Members

        public void Dispose()
        {
            _defaultContext.Dispose();
            _userContext.Dispose();
        }

        private async Task<CalcResult> createCalcTableTaskAsync(string attTable, string usTable, string calc)
        {
            string calcTable = string.Format("{0}_{1}_{2}", attTable, usTable, calc);
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
            _defaultContext.CreateModel(index);

            return await Task.Run(() =>
            {
                switch (calc)
                {
                    case CalcType.Sum:
                        _userContext.Procedures.SumTables(attTable, usTable, calcTable);
                        break;
                    case CalcType.Avg:
                        _userContext.Procedures.AvgTables(attTable, usTable, calcTable);
                        break;
                    default:
                        throw new Exception("Unknown calc type.");
                }

                index.Status = "Saving table.";
                index.Processed = true;
                _defaultContext.UpdateModel(index);

                _userContext.SaveTableToFile(calcTable);
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

                _defaultContext.UpdateModel(index);
                return result;
            });
        }
    }
}