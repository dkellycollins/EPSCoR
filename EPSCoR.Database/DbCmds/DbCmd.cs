using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EPSCoR.Database.Exceptions;
using EPSCoR.Database.Models;

namespace EPSCoR.Database.DbCmds
{
    /// <summary>
    /// This is the base class for all DbCmd classes. This class will mostly define helper functions for the sub classes to use.
    /// </summary>
    public abstract class DbCmd
    {
        protected DbContext _context;

        public DbCmd(DbContext context) 
        {
            _context = context;
        }

        internal abstract void AddTableFromFile(string file);
        internal abstract void PopulateTableFromFile(string file);
        public abstract void SumTables(string attTable, string usTable);
        public abstract void CreateDatabase(string databaseName);

        public virtual DataTable SelectAllFrom(string table)
        {
            ThrowExceptionIfInvalidSql(table);

            string query = "SELECT * FROM " + table;
            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(_context.Database.Connection);

            DbCommand cmd = dbFactory.CreateCommand();
            cmd.CommandText = query;
            cmd.Connection = _context.Database.Connection;

            DbDataAdapter dataAdapter = dbFactory.CreateDataAdapter();
            dataAdapter.SelectCommand = cmd;

            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);

            return dataTable;
        }

        public virtual void DropTable(string table)
        {
            ThrowExceptionIfInvalidSql(table);

            _context.Database.ExecuteSqlCommand("DROP TABLE " + table);
        }

        #region Helper Methods

        /// <summary>
        /// Throws an invalid file exception if any of the arguments contain an invalid character.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="args"></param>
        protected static void ThrowFileExceptionIfInvalidSql(string file, params string[] args)
        {
            foreach (string arg in args)
            {
                if (!Regex.IsMatch(arg.Trim(), @"^[a-zA-Z0-9_]+$", RegexOptions.IgnorePatternWhitespace))
                    throw new InvalidFileException(file, arg + " contains invalid characters");
            }
        }

        /// <summary>
        /// Throws an exception if any of the arguments contain an invalid character.
        /// </summary>
        /// <param name="args"></param>
        protected static void ThrowExceptionIfInvalidSql(params string[] args)
        {
            foreach (string arg in args)
            {
                if (!Regex.IsMatch(arg.Trim(), @"^[a-zA-Z0-9_]+$", RegexOptions.IgnorePatternWhitespace))
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

        #endregion Helper Methods
    }
}
