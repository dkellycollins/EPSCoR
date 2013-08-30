using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EPSCoR.Web.Database.Models
{
    /// <summary>
    /// The log table is used for general logging purposes.
    /// </summary>
    [Table("Log")]
    public class LogEntry : Model
    {
        /// <summary>
        /// The message for this log.
        /// </summary>
        [MaxLength(100)]
        public string Message { get; set; }

        /// <summary>
        /// If there was an error the error message.
        /// </summary>
        [MaxLength(100)]
        public string Error { get; set; }

        /// <summary>
        /// The program the made the log entry.
        /// </summary>
        public string Source { get; set; }
    }
}