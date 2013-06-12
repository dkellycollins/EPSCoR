using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPSCoR.Database.Models
{
    public class TableIndex : IModel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public string Name { get; set; }

        public string UploadedByUser { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public bool Processed { get; set; }
    }

    public class TableTypes
    {
        public const string ATTRIBUTE = "att";
        public const string UPSTREAM = "us";
        public const string CALC = "calc";
    }
}
