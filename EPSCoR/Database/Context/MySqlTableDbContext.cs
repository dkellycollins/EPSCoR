using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EPSCoR.Database.Exceptions;
using EPSCoR.Database.Services.Log;
using EPSCoR.Extentions;
using MySql.Data.MySqlClient;

namespace EPSCoR.Database.Context
{
    /// <summary>
    /// Performs database action using MySql commands
    /// </summary>
    internal class MySqlTableDbContext : TableDbContext
    {
        public MySqlTableDbContext()
            : base("MySqlConnection")
        { }

        public MySqlTableDbContext(MySqlConnection connection)
            : base(connection)
        { }

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
            List<string> fields = GetFieldsFromFile(file);
            if (fields.Count == 0)
                throw new InvalidFileException(file, "No data to process.");
            ThrowFileExceptionIfInvalidSql(file, fields.ToArray());

            //Build the column paramaters for the Sql query.
            string columns = fields.ToCommaSeparatedString("{0} double");
            //Make the first field the primary key.
            columns += ", PRIMARY KEY(" + fields[0] + ")";

            //Execute the command.
            string cmd = "CREATE TABLE IF NOT EXISTS " + tableName
                + " ( " + columns + " ) "
                + "ENGINE = InnoDB "
                + "DEFAULT CHARSET=latin1";
            Database.ExecuteSqlCommand(cmd);
            SaveChanges();
            LoggerFactory.GetLogger().Log("Table " + tableName + " added to " + Database.Connection.Database);
        }

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
            int rowsUpdated = Database.ExecuteSqlCommand(cmd);
            LoggerFactory.GetLogger().Log(rowsUpdated + " rows updated in table " + table + ", " + Database.Connection.Database);
        }

        internal override void SaveTableToFile(string table, string filePath)
        {
            IEnumerable<string> columns = getColumnsForTable(table);

            string cmd = "SELECT" + columns.ToCommaSeparatedString("'{0}'") + "UNION ALL" 
                + "(SELECT " + columns.ToCommaSeparatedString() + " INTO OUTFILE '" + filePath.Replace('\\', '/') + "'"
                + "FIELDS TERMINATED BY ',' OPTIONALLY ENCLOSED BY '\"' "
                + "LINES TERMINATED BY '\n' "
                + "FROM " + table + ")";
            Database.ExecuteSqlCommand(cmd);
            LoggerFactory.GetLogger().Log("Table, " + table + ", from database, " + Database.Connection.Database + ", saved to " + filePath);
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

        public override void DropTable(string table)
        {
            ThrowExceptionIfInvalidSql(table);

            Database.ExecuteSqlCommand("DROP TABLE IF EXISTS " + table);
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
            Database.ExecuteSqlCommand(cmd);
            SaveChanges();
            LoggerFactory.GetLogger().Log(calc + " table " + calcTable + "created in " + Database.Connection.Database);
        }

        private IEnumerable<string> getColumnsForTable(string table)
        {
            string showColumns = "SELECT column_name"
                               + " FROM information_schema.columns"
                               + " WHERE table_schema = '" + Database.Connection.Database + "'"
                               + " AND table_name = '" + table + "'";
            IEnumerable<string> columns = Database.SqlQuery<string>(showColumns);
            if (columns == null || columns.Count() == 0)
            {
                throw new Exception("Query to show fields from table failed");
            }
            return columns;
        }
    }
}
