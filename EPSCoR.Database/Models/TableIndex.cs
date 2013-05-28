using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSCoR.Database.Models
{
    public class TableIndex
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }
    }

    public class TableTypes
    {
        public const string ATTRIBUTE = "att";
        public const string UPSTREAM = "us";
        public const string CALC = "calc";
    }
}
