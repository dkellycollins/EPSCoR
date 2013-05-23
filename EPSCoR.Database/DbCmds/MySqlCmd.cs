using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPSCoR.Database.Exceptions;
using EPSCoR.Database.Models;
using EPSCoR.Database.Services.Log;

namespace EPSCoR.Database.DbCmds
{
    /// <summary>
    /// Performs database action using MySql commands
    /// </summary>
    internal class MySqlCmd
    {
        /// <summary>
        /// Creates a new table based on the file provided.
        /// </summary>
        /// <param name="file">CSV file.</param>
        /// <param name="dbContext">Reference to thte database.</param>
        public static void AddTableFromFile(string file)
        {
            //Get the table name.
            string tableName = Path.GetFileNameWithoutExtension(file);
            throwIfInvalidSql(tableName);

            //Get all the fields from the file.
            TextReader reader = File.OpenText(file);
            string head = reader.ReadLine();
            reader.Close();
            head = head.Replace('\"', ' ');
            string[] fields = head.Split(',');
            if (fields.Length == 0)
                throw new InvalidFileException(file, "No data to process.");
            throwIfInvalidSql(fields);

            //Build the column paramaters for the Sql query.
            StringBuilder columnsBuilder = new StringBuilder();
            for (int i = 0; i < fields.Count(); i++)
            {
                columnsBuilder.Append(fields[i] + " char(25), ");
            }
            //Make the first field the primary key.
            columnsBuilder.Append("PRIMARY KEY(" + fields[0] + ")");

            DefaultContext dbContext = DefaultContext.GetInstance();
            try
            {
                //Execute the command.
                dbContext.Database.ExecuteSqlCommand(
                    "CREATE TABLE IF NOT EXISTS " + tableName + " ( " + columnsBuilder.ToString() + " ) ENGINE = InnoDB DEFAULT CHARSET=latin1"
                    );
                LoggerFactory.Logger.Log("Table " + tableName + " added to the database.");
            }
            finally
            {
                DefaultContext.Release();
            } 
        }

        /// <summary>
        /// Populates the table with same name as the file with the data in the file.
        /// </summary>
        /// <param name="file">CSV file</param>
        /// <param name="dbContext">The reference to the database.</param>
        public static void PopulateTableFromFile(string file)
        {
            string table = Path.GetFileNameWithoutExtension(file);
            throwIfInvalidSql(file, table);

            DefaultContext dbContext = DefaultContext.GetInstance();
            try
            {
                string cmd = "LOAD DATA LOCAL INFILE '" + file.Replace('\\', '/') + "' INTO TABLE " + table + " FIELDS TERMINATED BY ',' OPTIONALLY ENCLOSED BY '\"' LINES TERMINATED BY '\n' IGNORE 1 LINES;";
                int rowsUpdated = dbContext.Database.ExecuteSqlCommand(cmd);
                LoggerFactory.Logger.Log(rowsUpdated + " rows updated in table " + table);
            }
            finally
            {
                DefaultContext.Release();
            }
            
        }

        /// <summary>
        /// Creates a new table where the two given tables have been summed.
        /// </summary>
        /// <param name="attTable">Attribute Table</param>
        /// <param name="usTable">Upstream Table</param>
        public static void SumTables(string attTable, string usTable)
        {
            throw new NotImplementedException();
            /*
            string calcTable = string.Format("{0}_{1}_calc", attTable, usTable);
            throwIfInvalidSql(attTable, usTable, calcTable);

            DefaultContext dbContext = DefaultContext.GetInstance();

            var rsult = dbContext.Database.ExecuteSqlCommand(
                "SHOW COLUMNS FROM {0}", 
                attTable,
                new SqlParameter("rsult", System.Data.SqlDbType.) { Direction = System.Data.ParameterDirection.ReturnValue });
            if(rsult == null)
                throw new Exception("Query to show fields from table failed.");

            var f_num = 0; // mysql_num_fields($rsult)
            var comm_f1 = "";
            var head_f1 = "";
            var comm_f2 = "";
            var head_f2 = "";

            if (0 > 0) //mysql_num_rows($rsult) > 0
            {
                var row = "";
	            while (false) //(row = mysql_fetch_row($rsult)) != null 
                { 
		            var r = row[0];
		            if (r != "ID" && r != "ARCID" && r != "OBJECTID" && r != "uni")
                    {
			            comm_f1 += ", SUM(" + r + ")";
			            head_f1 += ", " + r;

                        comm_f2 += ", " + r;
                        head_f2 += ", " + r;
		            }
	            }
	        }

            dbContext.Database.ExecuteSqlCommand(
                "SELECT POLYLINEID, ARCID, US_POLYID" + comm_f1
                    + "FROM (SELECT POLYLINEID, ARCID, US_POLYID" + comm_f2
		    	        + "FROM {1}, {2}"
		    	        + "WHERE ARCID = US_POLYID) Prod"
		            + "GROUP BY Prod.POLYLINEID"
		            + "LIMIT 40",
                    calcTable, attTable, usTable);

            DefaultContext.Release();
            Logger.Log("Created calc table " + calcTable);
             */
        }

        private static void throwIfInvalidSql(params string[] args)
        {
            //TODO implement
        }
    }
}
