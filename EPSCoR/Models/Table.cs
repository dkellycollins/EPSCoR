using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.Models
{
    public class Table
    {
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
        /// Filename of the upstream table.
        /// </summary>
        public string UpstreamTable { get; set; }

        /// <summary>
        /// Filename of the attribute table.
        /// </summary>
        public string AttributeTable { get; set; }

        public virtual UserProfile User { get; set; }
    }
}