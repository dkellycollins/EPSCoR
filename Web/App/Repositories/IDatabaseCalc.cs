using System;

namespace EPSCoR.Web.App.Repositories
{
    /// <summary>
    /// Provides functions for creating calc tables.
    /// </summary>
    public interface IDatabaseCalc : IDisposable
    {
        CalcResult JoinTables(string attTable, string usTable, string calcType);
        CalcResult SumTables(string attTable, string usTable);
        CalcResult AvgTables(string attTable, string usTalbe);
    }
}