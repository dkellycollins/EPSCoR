using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EPSCoR.Web.Database.Models
{
    [Table("Log")]
    public class LogEntry : Model
    {
        [MaxLength(100)]
        public string Message { get; set; }

        [MaxLength(100)]
        public string Error { get; set; }

        public string Source { get; set; }
    }
}