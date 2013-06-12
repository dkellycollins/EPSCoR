using System;

namespace EPSCoR.Database.Exceptions
{
    public class InvalidFileException : Exception
    {
        public string InvalidFile { get; set; }

        public InvalidFileException(string invalidFile, string message)
            : base(message)
        {
            InvalidFile = invalidFile;
        }
    }
}
