using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.Repositories
{

    public interface IRootRepository : IDisposable
    {
        void CreateDatabase(string databaseName);
        void DropDatabase(string databaseName);
    }
}