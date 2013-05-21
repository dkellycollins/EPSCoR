using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPSCoR.Database.Models;

namespace EPSCoR.Database
{
    /// <summary>
    /// Performs database action using MySql commands
    /// </summary>
    internal class MySql
    {
        /// <summary>
        /// Creates a new table based on the file provided.
        /// </summary>
        /// <param name="file">CSV file.</param>
        /// <param name="dbContext">Reference to thte database.</param>
        public static void AddTableFromFile(string file)
        {
            DefaultContext dbContext = DefaultContext.GetInstance();

            //Get all the fields from the file.
            TextReader reader = File.OpenText(file);
            string head = reader.ReadLine();
            reader.Close();
            head = head.Replace('\"', ' ');

            //Build the column paramaters for the Sql query.
            string[] fields = head.Split(',');
            if (fields.Length == 0)
                throw new InvalidFileException(file, "No data to process.");
            StringBuilder columnsBuilder = new StringBuilder();
            for (int i = 0; i < fields.Count(); i++)
            {
                columnsBuilder.Append(fields[i] + " char(25), ");
            }
            //Make the first field the primary key.
            columnsBuilder.Append("PRIMARY KEY(" + fields[0] + ")");

            string tableName = Path.GetFileNameWithoutExtension(file);
            dbContext.Database.ExecuteSqlCommand(
                "CREATE TABLE " + tableName + " (" + columnsBuilder.ToString() + ")"
                );

            DefaultContext.Release();
            Logger.Log("Table " + tableName + " added to the database.");
        }

        /// <summary>
        /// Populates the table with same name as the file with the data in the file.
        /// </summary>
        /// <param name="file">CSV file</param>
        /// <param name="dbContext">The reference to the database.</param>
        public static void PopulateTableFromFile(string file)
        {
            DefaultContext dbContext = DefaultContext.GetInstance();
            string table = Path.GetFileNameWithoutExtension(file);

            int rowsUpdated = dbContext.Database.ExecuteSqlCommand(
                "LOAD DATA LOCAL INFILE {0} INTO TABLE {1} FIELDS TERMINATED BY ',' OPTIONALLY ENCLOSED BY '\"' LINES TERMINATED BY '\n' IGNORE 1 LINES",
                file,
                table
                );

            DefaultContext.Release();
            Logger.Log(rowsUpdated + " rows updated in table " + table);
        }
    }
}
