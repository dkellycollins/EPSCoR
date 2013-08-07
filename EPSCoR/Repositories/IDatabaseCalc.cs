using System;

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

    public static class CalcType
    {
        public const string Sum = "SUM";
        public const string Avg = "AVG";
    }

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