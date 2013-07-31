using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPSCoR.Database.Models
{
    /// <summary>
    /// Model for an entry in the userprofile table.
    /// </summary>
    [Table("UserProfile")]
    public class UserProfile : IModel
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// When this entry was created.
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// When this entry was last updated.
        /// </summary>
        public DateTime DateUpdated { get; set; }

        /// <summary>
        /// The username.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The user's role on this site.
        /// </summary>
        public string Role { get; set; }
    }
}
