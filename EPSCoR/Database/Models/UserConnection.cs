using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.Database.Models
{
    public class UserConnection : IModel
    {
        public int ID { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public string User { get; set; }

        public string ConnectionId { get; set; }
    }
}