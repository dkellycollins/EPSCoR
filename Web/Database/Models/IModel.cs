using System;

namespace EPSCoR.Web.Database.Models
{
    /// <summary>
    /// Defines fields that every model will need to have.
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// The primary id for the model.
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// When the model was created.
        /// </summary>
        DateTime DateCreated { get; set; }

        /// <summary>
        /// When the model was last updated.
        /// </summary>
        DateTime DateUpdated { get; set; }
    }
}
