using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace EPSCoR
{
    public class PagedTable
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public DataTable Table { get; set; }
    }
}