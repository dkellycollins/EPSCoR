using System;

namespace EPSCoR.Common.Exceptions
{
    /// <summary>
    /// Should be thrown when there is an error processing a file.
    /// </summary>
    [Serializable]
    public class InvalidFileException : Exception
    {
        /// <summary>
        /// The fully quilified path to the file.
        /// </summary>
        public string InvalidFile { get; set; }

        public InvalidFileException(string invalidFile, string message)
            : base(message)
        {
            InvalidFile = invalidFile;
        }
    }
}
