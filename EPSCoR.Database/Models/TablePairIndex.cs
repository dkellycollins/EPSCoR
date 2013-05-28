using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EPSCoR.Database.Models
{
    public class TablePairIndex
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// Name of the tables.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Version of the tables.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Region of the tables.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Name of the upstream table.
        /// </summary>
        public string UpstreamTable { get; set; }

        /// <summary>
        /// Name of the attribute table.
        /// </summary>
        public string AttributeTable { get; set; }
    }
}