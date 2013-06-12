﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using EPSCoR.Database.Exceptions;
using EPSCoR.Database.Services.Log;

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
            string[] fields = GetFieldsFromFile(file);
            if (fields.Length == 0)
                throw new InvalidFileException(file, "No data to process.");
            ThrowFileExceptionIfInvalidSql(file, fields);

            //Build the column paramaters for the Sql query.
            StringBuilder columnsBuilder = new StringBuilder();
            for (int i = 0; i < fields.Count(); i++)
            {
                columnsBuilder.Append(fields[i] + " char(25), ");
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
            LoggerFactory.Log(rowsUpdated + " rows updated in table " + table + ", " + DatabaseName);
        }

        /// <summary>
        /// Creates a new table where the two given tables have been summed.
        /// </summary>
        /// <param name="attTable">Attribute Table</param>
        /// <param name="usTable">Upstream Table</param>
        public override void SumTables(string attTable, string usTable, string calcTable)
        {
            ThrowExceptionIfInvalidSql(attTable, usTable, calcTable);

            // Get columns
            string showColumns = "SHOW COLUMNS FROM " + attTable;
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
                if (column != "ID" && column != "ARCID" && column != "OBJECTID" && column != "uni")
                {
                    newColumns.Append(", SUM(" + column + ")");
                    curColumns.Append(", " + column);
                }
            }

            string cmd = "CREATE TABLE " + calcTable + " "
                + "SELECT POLYLINEID, ARCID, US_POLYID" + newColumns
                + "FROM (SELECT POLYLINEID, ARCID, US_POLYID" + curColumns + " "
                    + "FROM " + attTable + ", " + usTable + " "
                    + "WHERE ARCID = US_POLYID) Prod "
                + "GROUP BY Prod.POLYLINEID "
                + "LIMIT 40";
            _context.Database.ExecuteSqlCommand(cmd);
            _context.SaveChanges();
            LoggerFactory.Log("Sum table " + calcTable + "created in " + DatabaseName);
        }

        public override void CreateDatabase(string databaseName)
        {
            ThrowExceptionIfInvalidSql(databaseName);

            string cmd = "CREATE DATABASE " + databaseName;
            _context.Database.ExecuteSqlCommand(cmd);
            LoggerFactory.Log("Database " + databaseName + " added");
        }
    }
}
