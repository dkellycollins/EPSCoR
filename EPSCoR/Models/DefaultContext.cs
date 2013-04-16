using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EPSCoR.Models
{
    public class DefaultContext : DbContext
    {
        public DbSet<UserProfile> UserProfiles { get; set; }

        public DefaultContext()
            : base("DefaultConnection")
        {
        }
    }
}