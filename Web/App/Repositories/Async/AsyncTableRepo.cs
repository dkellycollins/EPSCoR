using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using EPSCoR.Common;
using EPSCoR.Web.Database;
using EPSCoR.Web.Database.Context;
using EPSCoR.Web.Database.Models;
using EPSCoR.Web.Database.Services;

namespace EPSCoR.Web.App.Repositories.Async
{
    public class AsyncTableRepo : IAsyncTableRepository, IAsyncDatabaseCalc
    {
        private ModelDbContext _modelContext;
        private TableDbContext _tableContext;
        private string _currentUser;

        public AsyncTableRepo(string userName)
        {
            _modelContext = DbContextFactory.GetModelDbContext();
            _tableContext = DbContextFactory.GetTableDbContextForUser(userName);
            _currentUser = userName;
        }

        #region IAsyncTableRepository Members

        public async Task CreateAsync(DataTable table)
        {
            throw new NotImplementedException();
        }

        public async Task<DataTable> ReadAsync(string tableName)
        {
            return await Task.Run(() => _tableContext.SelectAllFrom(tableName));
        }

        public async Task<DataTable> ReadAsync(string tableName, int lowerLimit, int upperLimit)
        {
            int totalRows = 0;
            return await Task.Run(() => _tableContext.SelectAllFrom(tableName, lowerLimit, upperLimit, out totalRows));
        }

        public async Task<int> CountAsync(string tableName)
        {
            return await Task.Run(() => _tableContext.Count(tableName));
        }

        public async Task UpdateAsync(DataTable table)
        {
            throw new NotImplementedException();
        }

        public async Task DropAsync(string tableName)
        {
            await Task.Run(() =>
            {
                _tableContext.DropTable(tableName);
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
            _modelContext.Dispose();
            _tableContext.Dispose();
        }

        private async Task<CalcResult> createCalcTableTaskAsync(string attTable, string usTable, string calc)
        {
            TableIndex index = null;
            Task calcTask = Task.Run(() =>
            {
                string calcTable = string.Format("{0}_{1}_{2}", attTable, usTable, calc);
                index = new TableIndex()
                {
                    Name = calcTable,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Status = "Generating table.",
                    Type = TableTypes.CALC,
                    UploadedByUser = _currentUser,
                    Processed = false
                };
                _modelContext.CreateModel(index);

                switch (calc)
                {
                    case CalcType.Sum:
                        _tableContext.SumTables(attTable, usTable, calcTable);
                        break;
                    case CalcType.Avg:
                        _tableContext.AvgTables(attTable, usTable, calcTable);
                        break;
                    default:
                        throw new Exception("Unknown calc type.");
                }

                index.Status = "Saving table.";
                _modelContext.UpdateModel(index);

                string calcTablePath = Path.Combine(DirectoryManager.ConversionDir, _currentUser, calcTable + ".csv");
                _tableContext.SaveTableToFile(calcTable, calcTablePath);
                index.FileKey = FileKeyGenerator.GenerateKey(calcTablePath);
            });
            Task<CalcResult> cleanupTask = calcTask.ContinueWith((task) =>
            {
                CalcResult result;
                if (task.IsFaulted)
                {
                    index.Status = "The server encountered an error while processing your request.";
                    index.Processed = false;
                    index.Error = true;
                    result = CalcResult.Error;
                }
                else if(task.IsCompleted)
                {
                    index.Status = "Table created.";
                    index.Processed = true;
                    index.Error = false;
                    result = CalcResult.Success;
                }
                else if (task.IsCanceled)
                {
                    index.Status = "The server was stopped while processing your request.";
                    index.Processed = false;
                    index.Error = false;
                    result = CalcResult.Unknown;
                }
                else
                {
                    result = CalcResult.Unknown;
                }

                _modelContext.UpdateModel(index);
                return result;
            });

            await calcTask;
            return await cleanupTask;
        }
    }
}