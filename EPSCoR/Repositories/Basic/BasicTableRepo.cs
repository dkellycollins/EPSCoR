﻿using System;
using System.Data;
using System.IO;
using System.Linq;
using EPSCoR.Database;
using EPSCoR.Database.Context;
using EPSCoR.Database.Models;
using EPSCoR.Database.Services;

namespace EPSCoR.Repositories.Basic
{
    public class BasicTableRepo : ITableRepository, IDatabaseCalc
    {
        ModelDbContext _modelContext;
        TableDbContext _tableContext;
        string _currentUser;

        public BasicTableRepo(string userName)
        {
            _modelContext = DbContextFactory.GetModelDbContext();
            _tableContext = DbContextFactory.GetTableDbContextForUser(userName);
            _currentUser = userName;
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
                return _tableContext.SelectAllFrom(tableName);
            }
            catch
            {
                return null;
            }
        }

        public DataTable Read(string tableName, int lowerLimit, int upperLimit)
        {
            try
            {
                int totalRows = 0;
                return _tableContext.SelectAllFrom(tableName, lowerLimit, upperLimit, out totalRows);
            }
            catch
            {
                return null;
            }
        }

        public int Count(string tableName)
        {
            return _tableContext.Count(tableName);
        }

        public void Update(DataTable table)
        {
            throw new NotImplementedException();
        }

        public void Drop(string tableName)
        {
            _tableContext.DropTable(tableName);

            TableIndex tableIndex = _modelContext.GetAllModels<TableIndex>().Where((t) => t.Name == tableName).FirstOrDefault();
            if (tableIndex != null)
            {
                _modelContext.RemoveModel(tableIndex);
            }
        }

        public void Dispose()
        {
            _modelContext.Dispose();
            _tableContext.Dispose();
        }

        #endregion IRawRepository Members

        #region IDatabaseCalc Members

        public CalcResult JoinTables(string attTable, string usTable, string calcType)
        {
            return createCalcTable(attTable, usTable, calcType);
        }

        public CalcResult SumTables(string attTable, string usTable)
        {
            return createCalcTable(attTable, usTable, CalcType.Sum);
        }

        public CalcResult AvgTables(string attTable, string usTable)
        {
            return createCalcTable(attTable, usTable, CalcType.Avg);
        }

        #endregion IDatabaseCalc Members

        private CalcResult createCalcTable(string attTable, string usTable, string calc)
        {
            string calcTable = string.Format("{0}_{1}_{2}", attTable, usTable, calc.ToString());
            TableIndex exisitingTable = _modelContext.GetAllModels<TableIndex>().Where(index => index.Name == calcTable && index.UploadedByUser == _currentUser).FirstOrDefault();
            if (exisitingTable != null)
                return CalcResult.TableAlreadyExists;

            try
            {
                TableIndex index = new TableIndex()
                {
                    Name = calcTable,
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
                        throw new Exception("Unknown calctype");
                }

                index.Status = "Saving table.";
                index.Processed = true;
                _modelContext.UpdateModel(index);

                string calcTablePath = Path.Combine(DirectoryManager.ConversionDir, _currentUser, calcTable + ".csv");
                _tableContext.SaveTableToFile(calcTable, calcTablePath);

                index.Status = "Table Created.";
                _modelContext.UpdateModel(index);

                return CalcResult.Success;
            }
            catch
            {
                return CalcResult.Error;
            }
        }
    }
}