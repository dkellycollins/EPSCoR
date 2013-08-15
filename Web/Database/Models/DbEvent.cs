using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSCoR.Web.Database.Models
{
    public class DbEvent : IModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public int ActionCode { get; set; }

        public string Table { get; set; }

        public int EntryID { get; set; }
    }

    public enum Action
    {
        Created = 1,
        Updated,
        Deleted
    }
}
