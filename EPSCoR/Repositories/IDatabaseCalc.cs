using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.Repositories
{
    /// <summary>
    /// The posible result for creating a calc table.
    /// </summary>
    public enum CalcResult
    {
        TableAlreadyExists,
        SubmittedForProcessing,
        Error,
        Success,
        Unknown
    }

    /// <summary>
    /// Provides functions for creating calc tables.
    /// </summary>
    public interface IDatabaseCalc : IDisposable
    {
        CalcResult SumTables(string attTable, string usTable);
        CalcResult AvgTables(string attTable, string usTalbe);
    }
}