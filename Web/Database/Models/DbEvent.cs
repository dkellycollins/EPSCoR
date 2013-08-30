using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSCoR.Web.Database.Models
{
    /// <summary>
    /// Represents an event that happened in the database.
    /// </summary>
    public class DbEvent : Model
    {
        /// <summary>
        /// The action that was performed on the Entry. This should match up to one of the Action values.
        /// </summary>
        public int ActionCode { get; set; }

        /// <summary>
        /// The table that was modified.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// The primary key of the entry.
        /// </summary>
        public int EntryID { get; set; }

        /// <summary>
        /// The program that made this change.
        /// </summary>
        public string Source { get; set; }
    }

    public enum Action
    {
        Created = 1,
        Updated,
        Deleted
    }
}
