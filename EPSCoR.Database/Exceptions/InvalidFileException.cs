using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPSCoR.Database.Exceptions
{
    class InvalidFileException : Exception
    {
        public string InvalidFile { get; set; }

        public InvalidFileException(string invalidFile, string message)
            : base(message)
        {
            InvalidFile = invalidFile;
        }
    }
}
