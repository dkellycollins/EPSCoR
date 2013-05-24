using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EPSCoR.Database.Exceptions;
using EPSCoR.Database.Models;
using EPSCoR.Database.Services.Log;

namespace EPSCoR.Database.DbCmds
{
    /// <summary>
    /// Performs database action using MySql commands
    /// </summary>
    internal class MySqlCmd : DbCmd
    {
        public MySqlCmd() : base() { }

        /// <summary>
        /// Creates a new table based on the file provided.
        /// </summary>
        /// <param name="file">CSV file.</param>
        /// <param name="dbContext">Reference to thte database.</param>
        public override void AddTableFromFile(string file)
        {
            //Get the table name.
            string tableName = Path.GetFileNameWithoutExtension(file);
            ThrowIfInvalidSql(tableName);

            //Get all the fields from the file.
            string[] fields = GetFieldsFromFile(file);
            if (fields.Length == 0)
                throw new InvalidFileException(file, "No data to process.");
            ThrowIfInvalidSql(file, fields);

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
                string cmd = "CREATE TABLE IF NOT EXISTS " + tableName
                    + " ( " + columnsBuilder.ToString() + " ) "
                    + "ENGINE = InnoDB "
                    + "DEFAULT CHARSET=latin1";
                dbContext.Database.ExecuteSqlCommand(cmd);
                dbContext.Set<TableIndex>().Add(new TableIndex()
                {
                    Name = tableName,
                    Type = (tableName.Contains("_ATT")) ? TableTypes.ATTRIBUTE : TableTypes.UPSTREAM
                });
                dbContext.SaveChanges();
                LoggerFactory.Log("Table " + tableName + " added to the database.");
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
        public override void PopulateTableFromFile(string file)
        {
            string table = Path.GetFileNameWithoutExtension(file);
            ThrowIfInvalidSql(file, table);

            DefaultContext dbContext = DefaultContext.GetInstance();
            try
            {
                string cmd = "LOAD DATA LOCAL INFILE '" + file.Replace('\\', '/') + "'"
                    + "INTO TABLE " + table + " "
                    + "FIELDS TERMINATED BY ','" 
                    + "OPTIONALLY ENCLOSED BY '\"'" 
                    + "LINES TERMINATED BY '\n'" 
                    + "IGNORE 1 LINES";
                int rowsUpdated = dbContext.Database.ExecuteSqlCommand(cmd);
                LoggerFactory.Log(rowsUpdated + " rows updated in table " + table);
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
        public override void SumTables(string attTable, string usTable)
        {
            throw new NotImplementedException();
            /*
            string calcTable = string.Format("{0}_{1}_calc", attTable, usTable);
            throwIfInvalidSql(attTable, usTable, calcTable);

            string[] fields = new string[]
            {
                "SUM(GRID_CODE)",
		        "SUM(LCOVV11)",
		        "SUM(LCOVV12)",
		        "SUM(LCOVV21)",
		        "SUM(LCOVV22)",
		        "SUM(LCOVV23)",
		        "SUM(LCOVV24)",
		        "SUM(LCOVV31)",
		        "SUM(LCOVV41)",
		        "SUM(LCOVV42)",
		        "SUM(LCOVV43)",
		        "SUM(LCOVV52)",
		        "SUM(LCOVV71)",
		        "SUM(LCOVV81)",
		        "SUM(LCOVV82)",
		        "SUM(LCOVV90)",
		        "SUM(LCOVV95)",
		        "SUM(LCOVV127)",
		        "SUM(Area)",
		        "SUM(HydroID_1)",
		        "SUM(LCOV11)",
		        "SUM(LCOV12)",
		        "SUM(LCOV21)",
		        "SUM(LCOV22)",
		        "SUM(LCOV23)",
		        "SUM(LCOV24)",
		        "SUM(LCOV31)",
		        "SUM(LCOV41)",
		        "SUM(LCOV42)",
		        "SUM(LCOV43)",
		        "SUM(LCOV52)",
		        "SUM(LCOV71)",
		        "SUM(LCOV81)",
		        "SUM(LCOV82)", 
		        "SUM(LCOV90)", 
		        "SUM(LCOV95)", 
		        "SUM(LCOV127)", 
		        "SUM(MUKEY)",
		        "SUM(slopegradd)",
		        "SUM(slopegradw)",
		        "SUM(slope_l)",
		        "SUM(slope_r)",
		        "SUM(slope_h)",
		        "SUM(tfact)",
		        "SUM(weg)",
		        "SUM(cokey)", 
		        "SUM(om_l)", 
		        "SUM(om_r)", 
		        "SUM(om_h)", 
		        "SUM(dbthirdbar)", 
		        "SUM(dbthirdb_1)", 
		        "SUM(dbthirdb_2)",
		        "SUM(ksat_l)", 
		        "SUM(ksat_r)", 
		        "SUM(ksat_h)",
		        "SUM(awc_l)", 
		        "SUM(awc_r)",
		        "SUM(awc_h)",
		        "SUM(kffact)",
		        "SUM(brock)",
		        "SUM(wt)",
		        "SUM(Fnode)",
		        "SUM(Tnode)",
		        "SUM(CatchID)",
		        "SUM(Strahler)",
		        "SUM(Segment)"
            };

            DefaultContext dbContext = DefaultContext.GetInstance();
            try
            {
                string cmd = "SELECT POLYLINEID, ARCID, US_POLYID" + comm_f1
                    + "FROM (SELECT POLYLINEID, ARCID, US_POLYID" + comm_f2
		    	        + "FROM " + attTable +  ", " + usTable + " "
		    	        + "WHERE ARCID = US_POLYID) Prod"
		            + "GROUP BY Prod.POLYLINEID"
		            + "LIMIT 40";
                dbContext.Database.ExecuteSqlCommand(cmd);
                dbContext.Set<TableIndex>().Add(new TableIndex()
                {
                    Name = calcTable,
                    Type = TableTypes.CALC
                });
                dbContext.SaveChanges();
                LoggerFactory.Logger.Log("Sum table " + calcTable + "created.");
            }
            finally
            {
                DefaultContext.Release();
            }
             * */
        }

        
    }
}
