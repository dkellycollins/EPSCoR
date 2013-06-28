using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using EPSCoR.Database.Exceptions;
using EPSCoR.Database.Services;
using EPSCoR.Database.Services.Log;
using MySql.Data.MySqlClient;
using EPSCoR.Database.Extentions;

namespace EPSCoR.Database.DbProcedure
{
    /// <summary>
    /// Performs database action using MySql commands
    /// </summary>
    internal class MySqlProcedures : DbProcedures
    {
        private string DatabaseName
        {
            get
            {
                return _context.Database.Connection.Database;
            }
        }

        public MySqlProcedures(DbContext context) 
            : base(context) 
        {
            if (!(context.Database.Connection is MySql.Data.MySqlClient.MySqlConnection))
                throw new Exception("Database connection is not a MySqlConnection");
        }

        /// <summary>
        /// Creates a new table based on the file provided.
        /// </summary>
        /// <param name="file">CSV file.</param>
        /// <param name="dbContext">Reference to thte database.</param>
        internal override void AddTableFromFile(string file)
        {
            //Get the table name.
            string tableName = Path.GetFileNameWithoutExtension(file);
            ThrowFileExceptionIfInvalidSql(file, tableName);

            //Get all the fields from the file.
            List<string> fields = new List<string>();
            List<string> samples = new List<string>();
            GetFieldsAndSamplesFromFile(file, fields, samples);
            if (fields.Count == 0 || samples.Count == 0)
                throw new InvalidFileException(file, "No data to process.");
            ThrowFileExceptionIfInvalidSql(file, fields.ToArray());

            //Build the column paramaters for the Sql query.
            StringBuilder columnsBuilder = new StringBuilder();
            foreach(string field in fields)
            {
                columnsBuilder.Append(field + " double, ");
            }
            //Make the first field the primary key.
            columnsBuilder.Append("PRIMARY KEY(" + fields[0] + ")");

            //Execute the command.
            string cmd = "CREATE TABLE IF NOT EXISTS " + tableName
                + " ( " + columnsBuilder.ToString() + " ) "
                + "ENGINE = InnoDB "
                + "DEFAULT CHARSET=latin1";
            _context.Database.ExecuteSqlCommand(cmd);
            _context.SaveChanges();
            LoggerFactory.GetLogger().Log("Table " + tableName + " added to " + DatabaseName);
        }

        private const int MAX_CMD_LENGTH = 1048576 / sizeof(char); //1mb divided by the size of a character.

        /// <summary>
        /// Populates the table with same name as the file with the data in the file.
        /// </summary>
        /// <param name="file">CSV file</param>
        /// <param name="dbContext">The reference to the database.</param>
        internal override void PopulateTableFromFile(string file)
        {
            string table = Path.GetFileNameWithoutExtension(file);
            ThrowFileExceptionIfInvalidSql(file, table);
            
            string cmd = "LOAD DATA LOCAL INFILE '" + file.Replace('\\', '/') + "'"
                    + "INTO TABLE " + table + " "
                    + "FIELDS TERMINATED BY ','"
                    + "OPTIONALLY ENCLOSED BY '\"'"
                    + "LINES TERMINATED BY '\n'"
                    + "IGNORE 1 LINES";
            int rowsUpdated = _context.Database.ExecuteSqlCommand(cmd);
            LoggerFactory.GetLogger().Log(rowsUpdated + " rows updated in table " + table + ", " + DatabaseName);
        }

        internal override void SaveTableToFile(string table, string filePath)
        {
            IEnumerable<string> columns = getColumnsForTable(table);
            string strColumns = columns.ToCommaSeparatedString("'{0}'");

            string cmd = "SELECT " + strColumns + " INTO OUTFILE '" + filePath.Replace('\\', '/') + "'"
                + "FIELDS TERMINATED BY ',' OPTIONALLY ENCLOSED BY '\"' "
                + "LINES TERMINATED BY '\n' "
                + "FROM " + table;
            _context.Database.ExecuteSqlCommand(cmd);
            LoggerFactory.GetLogger().Log("Table, " + table + ", from database, " + DatabaseName + ", saved to " + filePath);
        }

        /// <summary>
        /// Creates a new table where the two given tables have been summed.
        /// </summary>
        /// <param name="attTable">Attribute Table</param>
        /// <param name="usTable">Upstream Table</param>
        public override void SumTables(string attTable, string usTable, string calcTable)
        {
            createCalcTable(attTable, usTable, calcTable, "SUM");
        }

        public override void AvgTables(string attTable, string usTable, string calcTable)
        {
            createCalcTable(attTable, usTable, calcTable, "AVG");
        }

        private void createCalcTable(string attTable, string usTable, string calcTable, string calc)
        {
            ThrowExceptionIfInvalidSql(attTable, usTable, calcTable);            

            // Get columns
            IEnumerable<string> columns = getColumnsForTable(attTable);

            //Build two strings. One that has each column name separated by commas and once that has each column name wrapped in SUM()
            StringBuilder newColumns = new StringBuilder();
            StringBuilder curColumns = new StringBuilder();
            foreach (string column in columns)
            {
                //TODO make this if statement dynamic
                if (column != "ID" && column != "ARCID" && column != "OBJECTID" && column != "uni")
                {
                    newColumns.Append(string.Format(", {0}({1}) AS {0}_{1}", calc, column));
                    curColumns.Append(", " + column);
                }
            }

            string cmd = "CREATE TABLE " + calcTable + " "
                + "SELECT POLYLINEID, ARCID, US_POLYID" + newColumns.ToString() + " "
                + "FROM ("
                    + "SELECT POLYLINEID, ARCID, US_POLYID" + curColumns.ToString() + " "
                    + "FROM " + attTable + ", " + usTable + " "
                    + "WHERE ARCID = US_POLYID"
                + ") as Prod "
                + "GROUP BY Prod.POLYLINEID ";
            _context.Database.ExecuteSqlCommand(cmd);
            _context.SaveChanges();
            LoggerFactory.GetLogger().Log(calc + " table " + calcTable + "created in " + DatabaseName);
        }

        private IEnumerable<string> getColumnsForTable(string table)
        {
            string showColumns = "SELECT column_name"
                               + " FROM information_schema.columns"
                               + " WHERE table_schema = '" + DatabaseName + "'"
                               + " AND table_name = '" + table + "'";
            IEnumerable<string> columns = _context.Database.SqlQuery<string>(showColumns);
            if (columns == null || columns.Count() == 0)
            {
                throw new Exception("Query to show fields from table failed");
            }
            return columns;
        }
    }
}
