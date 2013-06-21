using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace EPSCoR.Repositories
{
    public struct ReadParam
    {
        public int UpperLimit { get; set; }
        public int LowerLimit { get; set; }
    }

    public interface ITableRepository : IDisposable
    {
        void Create(DataTable table);
        DataTable Read(string tableName);
        DataTable Read(string tableName, int lowerLimit, int upperLimit);
        int Count(string tableName);
        void Update(DataTable table);
        void Drop(string tableName);
    }

    public enum CalcResult
    {
        TableAlreadyExists,
        SubmittedForProcessing,
        Error,
        Success,
        Unknown
    }

    public interface IDatabaseCalc : IDisposable
    {
        CalcResult SumTables(string attTable, string usTable);
        CalcResult AvgTables(string attTable, string usTalbe);
    }
}