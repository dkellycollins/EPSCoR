using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.Common.Exceptions
{
    /// <summary>
    /// Should be thrown when input for an sql query is Invalid.
    /// </summary>
    public class InvalidSqlException : Exception
    {
        /// <summary>
        /// The query or input that is invalid.
        /// </summary>
        public string InvalidSql { get; private set; }

        public InvalidSqlException(string message, string query)
            : base(message)
        {
            InvalidSql = query;
        }
    }
}