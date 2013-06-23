using System;

namespace EPSCoR.Database.Exceptions
{
    /// <summary>
    /// Should be thrown when there is an error processing a file.
    /// </summary>
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
