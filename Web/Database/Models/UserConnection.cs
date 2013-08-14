using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EPSCoR.Web.Database.Models
{
    public class UserConnection : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        [MaxLength(45)]
        public string User { get; set; }

        [MaxLength(45)]
        public string ConnectionId { get; set; }
    }
}