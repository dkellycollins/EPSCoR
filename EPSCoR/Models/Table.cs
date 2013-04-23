using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.Models
{
    public class Table
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public virtual UserProfile User { get; set; }
    }
}