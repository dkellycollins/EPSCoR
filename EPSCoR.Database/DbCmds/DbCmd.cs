using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EPSCoR.Database.Exceptions;

namespace EPSCoR.Database.DbCmds
{
    /// <summary>
    /// This is the base class for all DbCmd classes. This class will mostly define helper functions for the sub classes to use.
    /// </summary>
    internal abstract class DbCmd
    {
        public DbCmd() { }

        public abstract void AddTableFromFile(string file);
        public abstract void PopulateTableFromFile(string file);
        public abstract void SumTables(string attTable, string usTable);

        /// <summary>
        /// Throws an invalid file exception if any of the arguments contain an invalid character.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="args"></param>
        protected static void ThrowIfInvalidSql(string file, params string[] args)
        {
            foreach (string arg in args)
            {
                if (!Regex.IsMatch(arg, @"^[a-zA-Z0-9_]+$"))
                    throw new InvalidFileException(file, arg + " contains invalid characters");
            }
        }

        /// <summary>
        /// Throws an exception if any of the arguments contain an invalid character.
        /// </summary>
        /// <param name="args"></param>
        protected static void ThrowIfInvalidSql(params string[] args)
        {
            foreach (string arg in args)
            {
                if (!Regex.IsMatch(arg, @"^[a-zA-Z0-9_]+$"))
                    throw new Exception(arg + " contains invalid characters");
            }
        }

        protected static string[] GetFieldsFromFile(string file)
        {
            TextReader reader = File.OpenText(file);
            string head = reader.ReadLine();
            reader.Close();
            head = head.Replace('\"', ' ');
            return head.Split(',');
        }
    }
}
