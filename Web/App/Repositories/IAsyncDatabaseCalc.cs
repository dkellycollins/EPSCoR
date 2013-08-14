using System;
using System.Threading.Tasks;

namespace EPSCoR.Web.App.Repositories
{
    /// <summary>
    /// Provides functions for creating calc tables.
    /// </summary>
    public interface IAsyncDatabaseCalc : IDisposable
    {
        Task<CalcResult> JoinTablesAsync(string attTable, string usTable, string calcType);
        Task<CalcResult> SumTablesAsync(string attTable, string usTable);
        Task<CalcResult> AvgTablesAsync(string attTable, string usTalbe);
    }
}