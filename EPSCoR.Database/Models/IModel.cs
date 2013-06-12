using System;

namespace EPSCoR.Database.Models
{
    public interface IModel
    {
        int ID { get; set; }

        DateTime DateCreated { get; set; }

        DateTime DateUpdated { get; set; }
    }
}
