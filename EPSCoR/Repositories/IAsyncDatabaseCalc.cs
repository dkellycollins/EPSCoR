using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EPSCoR.Repositories
{
    /// <summary>
    /// Provides functions for creating calc tables.
    /// </summary>
    public interface IAsyncDatabaseCalc : IDisposable
    {
        Task<CalcResult> SumTablesAsync(string attTable, string usTable);
        Task<CalcResult> AvgTablesAsync(string attTable, string usTalbe);
    }
}