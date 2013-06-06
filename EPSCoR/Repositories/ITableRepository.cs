using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace EPSCoR.Repositories
{
    public interface ITableRepository : IDisposable
    {
        void Create(DataTable table);
        DataTable Read(string tableName);
        void Update(DataTable table);
        void Drop(string tableName);
    }

    public interface IDatabaseCalc : IDisposable
    {
        void SumTables(string attTable, string usTable);
        void AvgTables(string attTable, string usTalbe);
    }
}