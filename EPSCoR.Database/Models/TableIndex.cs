using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSCoR.Database.Models
{
    public class TableIndex : ModelBase
    {
        public string Name { get; set; }

        public string UploadedByUser { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public string GeTableName()
        {
            return string.Format("{0}_{1}", UploadedByUser, Name);
        }
    }

    public class TableTypes
    {
        public const string ATTRIBUTE = "att";
        public const string UPSTREAM = "us";
        public const string CALC = "calc";
    }
}
