using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPSCoR.Database.Models;

namespace EPSCoR.Database
{
    /// <summary>
    /// Performs database actions using MsSql commands.
    /// </summary>
    internal class SqlServerCmd
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

            dbContext.Database.ExecuteSqlCommand(
                "CREATE TABLE " + Path.GetFileNameWithoutExtension(file) + " (" + columnsBuilder.ToString() + ")"
                );

            DefaultContext.Release();
            Logger.Log("Table " + Path.GetFileNameWithoutExtension(file) + " added to the database.");
        }

        /// <summary>
        /// Populates the table with same name as the file with the data in the file.
        /// </summary>
        /// <param name="file">CSV file</param>
        /// <param name="dbContext">The reference to the database.</param>
        public static void PopulateTableFromFile(string file)
        {
            throw new NotImplementedException();
        }
    }
}
