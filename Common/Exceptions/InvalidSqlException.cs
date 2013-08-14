using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.Common.Exceptions
{
    public class InvalidSqlException : Exception
    {
        public string InvalidSql { get; private set; }

        public InvalidSqlException(string message, string query)
            : base(message)
        {
            InvalidSql = query;
        }
    }
}