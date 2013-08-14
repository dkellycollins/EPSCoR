using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EPSCoR.Common.Exceptions;

namespace EPSCoR.Web.Database.Context
{
    /// <summary>
    /// This is the base class for all DbCmd classes. This class will mostly define helper functions for the sub classes to use.
    /// </summary>
    public abstract class TableDbContext : DbContext
    {
        public TableDbContext()
        { }

        public TableDbContext(string connectionString)
            : base(connectionString)
        { }

        public TableDbContext(DbConnection connection)
            : base(connection, false)
        { }

        /// <summary>
        /// Adds an empty table using the given file.
        /// </summary>
        /// <param name="file"></param>
        public abstract void AddTableFromFile(string file);

        /// <summary>
        /// Populates the table using the given file.
        /// </summary>
        /// <param name="file"></param>
        public abstract void PopulateTableFromFile(string file);

        /// <summary>
        /// Saves the table to a csv file.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="file"></param>
        public abstract void SaveTableToFile(string table, string file);

        /// <summary>
        /// Creates a new table that performs a sum on join table of the attTable and the usTable.
        /// </summary>
        /// <param name="attTable">Name of the Attribute table to use.</param>
        /// <param name="usTable">Name of the Upstream table to use.</param>
        /// <param name="calcTable">Name of the calc table to create.</param>
        public abstract void SumTables(string attTable, string usTable, string calcTable);

        /// <summary>
        /// Creates a new table that performs an average on join table of the attTable and the usTable.
        /// </summary>
        /// <param name="attTable">Name of the Attribute table to use.</param>
        /// <param name="usTable">Name of the Upstream table to use.</param>
        /// <param name="calcTable">Name of the calc table to create.</param>
        public abstract void AvgTables(string attTable, string usTable, string calcTable);

        /// <summary>
        /// Selects everything from a table and returns it in a datatable object.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public virtual DataTable SelectAllFrom(string table)
        {
            ThrowExceptionIfInvalidSql(table);

            string query = "SELECT * FROM " + table;
            return CommonSelect(query);
        }

        public virtual DataTable SelectAllFrom(string table, int lowerLimit, int upperLimit, out int totalRows)
        {
            ThrowExceptionIfInvalidSql(table);

            string mainQuery = "SELECT SQL_CALC_FOUND_ROWS * FROM " + table + " "
                        + "LIMIT " + lowerLimit + ", " + upperLimit;
            string rowsQuery = "SELECT FOUND_ROWS()";
            
            DataTable result = CommonSelect(mainQuery);
            totalRows = Database.ExecuteSqlCommand(rowsQuery);
            
            return result;
        }

        public virtual int Count(string table)
        {
            ThrowExceptionIfInvalidSql(table);

            string query = "SELECT COUNT(*) FROM " + table;
            return Database.SqlQuery<int>(query).FirstOrDefault();
        }

        protected virtual DataTable CommonSelect(string query)
        {
            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(Database.Connection);

            DbCommand cmd = dbFactory.CreateCommand();
            cmd.CommandText = query;
            cmd.Connection = Database.Connection;

            DbDataAdapter dataAdapter = dbFactory.CreateDataAdapter();
            dataAdapter.SelectCommand = cmd;

            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);

            return dataTable;
        }

        public virtual void DropTable(string table)
        {
            ThrowExceptionIfInvalidSql(table);

            Database.ExecuteSqlCommand("DROP TABLE " + table);
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

        protected static List<string> GetFieldsFromFile(string file)
        {
            List<string> fields = new List<string>();
            GetFieldsAndSamplesFromFile(file, fields, null);
            return fields;
        }

        protected static List<string> GetSampleFromFile(string file)
        {
            List<string> samples = new List<string>();
            GetFieldsAndSamplesFromFile(file, null, samples);
            return samples;
        }

        protected static void GetFieldsAndSamplesFromFile(string file, List<string> fields, List<string> samples)
        {
            using (TextReader reader = File.OpenText(file))
            {
                string head = reader.ReadLine();
                if (fields != null)
                {
                    head = head.Replace('\"', ' ');
                    fields.AddRange(head.Split(','));
                }

                string samp = reader.ReadLine();
                if (samples != null)
                {
                    samp = samp.Replace('\"', ' ');
                    samples.AddRange(samp.Split(','));
                }
            }
        }

        #endregion Helper Methods
    }
}
