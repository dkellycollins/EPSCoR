using EPSCoR.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EPSCoR.Repositories
{
    public class DefaultContext : DbContext
    {
        private static DefaultContext _instance;
        public static DefaultContext Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DefaultContext();
                return _instance;
            }
        } 

        public DbSet<TableIndex> Tables { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        public DefaultContext()
            : base("DefaultConnection")
        {
        }
    }
}