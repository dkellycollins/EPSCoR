using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EPSCoR.Web.Database.Models
{
    public class UserConnection : Model
    {
        [MaxLength(45)]
        public string User { get; set; }

        [MaxLength(45)]
        public string ConnectionId { get; set; }
    }
}