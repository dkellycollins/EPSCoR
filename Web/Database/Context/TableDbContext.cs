using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EPSCoR.Common.Exceptions;
using EPSCoR.Web.Database.Services.Log;

namespace EPSCoR.Web.Database.Context
{
    /// <summary>
    /// Extends DbContext to allow access to the full tables in the database.
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

        /// <summary>
        /// Selected the given range from the table and return the result in a DataTable object.
        /// </summary>
        /// <param name="table">The table to select from.</param>
        /// <param name="lowerLimit">The lower limit of the range.</param>
        /// <param name="upperLimit">The upper limit of the range.</param>
        /// <param name="totalRows">How many rows were returns from the complete query.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns how many rows are in the given table.
        /// </summary>
        /// <param name="table">Name of the table.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Drops the given table.
        /// </summary>
        /// <param name="table">Name of the table to drop.</param>
        public virtual void DropTable(string table)
        {
            ThrowExceptionIfInvalidSql(table);

            Database.ExecuteSqlCommand("DROP TABLE " + table);
            LoggerFactory.GetLogger().Log("Table, " + table + ", dropped from " + Database.Connection.Database);
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
                    throw new InvalidSqlException("Query contains invalid characters", arg);
            }
        }

        /// <summary>
        /// Determines the fields for a table from the first row in the file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected static List<string> GetFieldsFromFile(string file)
        {
            List<string> fields = new List<string>();
            GetFieldsAndSamplesFromFile(file, fields, null);
            return fields;
        }

        /// <summary>
        /// Returns the second row from the file, tis should be a "sample" of the data
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        protected static List<string> GetSampleFromFile(string file)
        {
            List<string> samples = new List<string>();
            GetFieldsAndSamplesFromFile(file, null, samples);
            return samples;
        }

        /// <summary>
        /// Returns the first row as fields and the second row as samples.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fields"></param>
        /// <param name="samples"></param>
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
