using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.ViewModels
{
    public class TableIndexVM
    {
        public List<string> Tables;
        public CalcFormVM CalcForm;
        public List<ConvertedTablesVM> ConvertedTables;
    }

    public class CalcFormVM
    {
        public List<string> AttributeTables;
        public List<string> UpstreamTables;
    }

    public class ConvertedTablesVM
    {
        public int TableID;
        public string Table;
        public string Type;
        public string Time;
        public string Issuer;
    }
}