using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EPSCoR.ViewModels
{
    public class FileDownloadVM
    {
        public string Table { get; set; }
        public string Type { get; set; }
        public string DateCreated { get; set; }
        public string Issuer { get; set; }
    }
}