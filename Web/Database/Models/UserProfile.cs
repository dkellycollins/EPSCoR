using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPSCoR.Web.Database.Models
{
    /// <summary>
    /// Model for an entry in the userprofile table.
    /// </summary>
    [Table("UserProfile")]
    public class UserProfile : Model
    {
        /// <summary>
        /// The username.
        /// </summary>
        [MaxLength(25)]
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// The user's role on this site.
        /// </summary>
        [MaxLength(25)]
        public string Role { get; set; }
    }
}
