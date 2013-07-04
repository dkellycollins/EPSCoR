﻿using EPSCoR.Database;
using EPSCoR.Database.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EPSCoR.Repositories.Async
{
    public class AsyncTableRepo : ITableRepository, IAsyncTableRepository, IDatabaseCalc, IAsyncDatabaseCalc
    {
        DefaultContext _defaultContext;
        UserContext _userContext;
        string currentUser;

        public AsyncTableRepo(string userName)
        {
            _defaultContext = new DefaultContext();
            _userContext = UserContext.GetContextForUser(userName);
            currentUser = userName;
        }

        #region ITableRepository Members

        public void Create(DataTable table)
        {
            throw new NotImplementedException();
        }

        public DataTable Read(string tableName)
        {
            return ReadAsync(tableName).Result;
        }

        public DataTable Read(string tableName, int lowerLimit, int upperLimit)
        {
            return ReadAsync(tableName, lowerLimit, upperLimit).Result;
        }

        public int Count(string tableName)
        {
            return CountAsync(tableName).Result;
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

        #endregion ITableRepository Members

        #region IAsyncTableRepository Members

        public async void CreateAsync(DataTable table)
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

        public async void UpdateAsync(DataTable table)
        {
            throw new NotImplementedException();
        }

        public async void DropAsync(string tableName)
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

        #endregion IAsyncTableRepository

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

        #region IAysncDatabaseCalc Members

        public Task<CalcResult> SumTablesAysnc(string attTable, string usTable)
        {
            return createCalcTableTaskAsync(attTable, usTable, CalcType.SUM);
        }

        public Task<CalcResult> AvgTablesAsync(string attTable, string usTable)
        {
            return createCalcTableTaskAsync(attTable, usTable, CalcType.AVG);
        }

        #endregion IAsyncDatabaseCalc Members

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