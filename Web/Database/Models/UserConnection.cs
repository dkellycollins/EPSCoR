using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace EPSCoR.Web.Database.Models
{
    /// <summary>
    /// Stores the connection id for signalR hubs.
    /// </summary>
    public class UserConnection : Model
    {
        /// <summary>
        /// The username for the connection.
        /// </summary>
        [MaxLength(45)]
        public string User { get; set; }

        /// <summary>
        /// A unquie id for this connection.
        /// </summary>
        [MaxLength(45)]
        public string ConnectionId { get; set; }
    }
}