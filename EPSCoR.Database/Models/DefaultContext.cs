using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using EPSCoR.Database.DbCmds;
using MySql.Data.MySqlClient;

namespace EPSCoR.Database.Models
{
    public class DefaultContext : DbContext
    {
        private static DefaultContext _instance = null;
        private static int referenceCount = 0;

        public static DefaultContext GetInstance()
        {
            Interlocked.Increment(ref referenceCount);
            if (referenceCount == 1)
            {
                _instance = new DefaultContext();
            }
            return _instance;
        }

        public static void Release()
        {
            Interlocked.Decrement(ref referenceCount);
            if (referenceCount == 0)
            {
                _instance.Dispose();
                _instance = null;
            }
        }

        public DbSet<TableIndex> Tables { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbCmd Commands { get; private set; }

        public DefaultContext()
            : base("MySqlConnection")
        {
            Commands = new MySqlCmd(this);
        }
    }
}