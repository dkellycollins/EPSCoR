using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace EPSCoR.Repositories
{
    public interface IRawRepository : IDisposable
    {
        DataTable Get(string tableName);
        void Drop(string tableName);
    }
}