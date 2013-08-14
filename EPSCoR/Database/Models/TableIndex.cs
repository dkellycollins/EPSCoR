using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace EPSCoR.Database.Models
{
    /// <summary>
    /// Model for entries in the tableindexes table.
    /// </summary>
    public class TableIndex : IModel
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int ID { get; set; }

        /// <summary>
        /// When the entry was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// When the entry was last updated.
        /// </summary>
        public DateTime DateUpdated { get; set; }

        /// <summary>
        /// The name of the table.
        /// </summary>
        [MaxLength(25)]
        public string Name { get; set; }

        /// <summary>
        /// Who uploaded this table.
        /// </summary>
        [MaxLength(25)]
        public string UploadedByUser { get; set; }

        /// <summary>
        /// The type of table this table is. Should be one if the values in tabletypes.
        /// </summary>
        [MaxLength(10)]
        public string Type { get; set; }

        /// <summary>
        /// The current status of the table.
        /// </summary>
        [MaxLength(50)]
        public string Status { get; set; }

        /// <summary>
        /// True if the table has been fully processed.
        /// </summary>
        public bool Processed { get; set; }

        /// <summary>
        /// If true then an error occured while processing the table. An actual table may or may not exist
        /// </summary>
        public bool Error { get; set; }

        /// <summary>
        /// The number of rows the table contains.
        /// </summary>
        //public int NumRows { get; set; }

        /// <summary>
        /// If true then this table can be accessed by all users.
        /// </summary>
        //public bool Shared { get; set; }

        [MaxLength(32)]
        [JsonIgnore]
        public string FileKey { get; set; }
    }

    /// <summary>
    /// Defines the types of tables we can have.
    /// </summary>
    public class TableTypes
    {
        public const string ATTRIBUTE = "att";
        public const string UPSTREAM = "us";
        public const string CALC = "calc";
    }
}
