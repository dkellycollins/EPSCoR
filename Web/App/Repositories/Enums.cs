using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.Web.App.Repositories
{
    public enum FileDirectory
    {
        Temp,
        Upload,
        Conversion,
        Archive,
        Invalid
    }

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
    /// The different type of calculation that can be performed.
    /// </summary>
    public static class CalcType
    {
        public const string Sum = "SUM";
        public const string Avg = "AVG";
    }
}