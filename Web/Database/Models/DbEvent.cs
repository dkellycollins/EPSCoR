using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPSCoR.Web.Database.Models
{
    public class DbEvent : Model
    {
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
