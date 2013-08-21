using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace EPSCoR.Web.Database.Models
{
    /// <summary>
    /// Defines fields that every model will need to have.
    /// </summary>
    public class Model
    {
        /// <summary>
        /// The primary id for the model.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public int ID { get; set; }

        /// <summary>
        /// When the model was created.
        /// </summary>
        [JsonIgnore]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// When the model was last updated.
        /// </summary>
        [JsonIgnore]
        public DateTime DateUpdated { get; set; }
    }
}
