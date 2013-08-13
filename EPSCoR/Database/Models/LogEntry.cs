using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EPSCoR.Database.Models
{
    [Table("Log")]
    public class LogEntry : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        [MaxLength(100)]
        public string Message { get; set; }

        [MaxLength(100)]
        public string Error { get; set; }
    }
}