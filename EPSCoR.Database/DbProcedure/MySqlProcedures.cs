using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using EPSCoR.Database.Exceptions;
using EPSCoR.Database.Services.Log;
using MySql.Data.MySqlClient;

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
            for (int i = 0; i < fields.Count(); i++)
            {
                string type = getValueType(samples[i]);
                columnsBuilder.Append(fields[i] + " " + type + ", ");
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
            LoggerFactory.Log("Table " + tableName + " added to " + DatabaseName);
        }

        private string getValueType(string sample)
        {
            int x;
            double y;
            //if (Int32.TryParse(sample, out x))
            //    return "int";
            if (Double.TryParse(sample, out y))
                return "double";
            return "varchar(25)";
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

            /*
            InsertCmd insertCmd = new InsertCmd(table);
            using (TextReader reader = File.OpenText(file))
            {
                //Build the column paramaters for the Sql query.
                string head = reader.ReadLine();
                head = head.Replace('\"', ' ');
                string[] fields = head.Split(',');
                insertCmd.AddColumns(fields);

                //Build the values string.
                string buf = string.Empty;
                while ((buf = reader.ReadLine()) != null)
                {
                    string[] values = buf.Split(',');
                    insertCmd.AddRow(values);
                }
            }

            int rowsUpdated = insertCmd.ExecuteCmd(_context, MAX_CMD_LENGTH);
            */
            
            
            string cmd = "LOAD DATA LOCAL INFILE '" + file.Replace('\\', '/') + "'"
                    + "INTO TABLE " + table + " "
                    + "FIELDS TERMINATED BY ','"
                    + "OPTIONALLY ENCLOSED BY '\"'"
                    + "LINES TERMINATED BY '\n'"
                    + "IGNORE 1 LINES";
            int rowsUpdated = _context.Database.ExecuteSqlCommand(cmd);
            LoggerFactory.Log(rowsUpdated + " rows updated in table " + table + ", " + DatabaseName);
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
            string showColumns = "SELECT column_name"
                                + " FROM information_schema.columns"
                                + " WHERE table_schema = '" + _context.Database.Connection.Database + "'"
                                + " AND table_name = '" + attTable + "'";
            IEnumerable<string> columns = _context.Database.SqlQuery<string>(showColumns);
            if (columns == null || columns.Count() == 0)
            {
                throw new Exception("Query to show fields from table failed");
            }

            //Build two strings. One that has each column name separated by commas and once that has each column name wrapped in SUM()
            StringBuilder newColumns = new StringBuilder();
            StringBuilder curColumns = new StringBuilder();
            foreach (string column in columns)
            {
                //TODO make this if statement dynamic
                if (column != "ID" && column != "ARCID" && column != "OBJECTID" && column != "uni")
                {
                    newColumns.Append(string.Format(", {0}({1})", calc, column));
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
            LoggerFactory.Log(calc + " table " + calcTable + "created in " + DatabaseName);
        }
    }
}
