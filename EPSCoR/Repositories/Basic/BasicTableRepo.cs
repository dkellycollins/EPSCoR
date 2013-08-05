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
        ModelDbContext _defaultContext;
        UserContext _userContext;
        string currentUser;

        public BasicTableRepo(string userName)
        {
            _defaultContext = new ModelDbContext();
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

        public DataTable Read(string tableName, int lowerLimit, int upperLimit)
        {
            try
            {
                int totalRows = 0;
                return _userContext.Procedures.SelectAllFrom(tableName, lowerLimit, upperLimit, out totalRows);
            }
            catch
            {
                return null;
            }
        }

        public int Count(string tableName)
        {
            return _userContext.Procedures.Count(tableName);
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
                _defaultContext.RemoveModel(tableIndex);
            }
        }

        public void Dispose()
        {
            _defaultContext.Dispose();
            _userContext.Dispose();
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
            TableIndex exisitingTable = _defaultContext.GetTableIndex(calcTable, currentUser);
            if (exisitingTable != null)
                return CalcResult.TableAlreadyExists;

            try
            {
                TableIndex index = new TableIndex()
                {
                    Name = calcTable,
                    Status = "Generating table.",
                    Type = TableTypes.CALC,
                    UploadedByUser = currentUser,
                    Processed = false
                };
                _defaultContext.CreateModel(index);

                switch (calc)
                {
                    case CalcType.Sum:
                        _userContext.Procedures.SumTables(attTable, usTable, calcTable);
                        break;
                    case CalcType.Avg:
                        _userContext.Procedures.AvgTables(attTable, usTable, calcTable);
                        break;
                    default:
                        throw new Exception("Unknown calctype");
                }

                index.Status = "Saving table.";
                index.Processed = true;
                _defaultContext.UpdateModel(index);

                _userContext.SaveTableToFile(calcTable);

                index.Status = "Table Created.";
                _defaultContext.UpdateModel(index);

                return CalcResult.Success;
            }
            catch
            {
                return CalcResult.Error;
            }
        }
    }
}