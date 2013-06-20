using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using EPSCoR.Database.Exceptions;
using EPSCoR.Database.Services.Log;

namespace EPSCoR.Database.DbProcedure
{
    /// <summary>
    /// Performs database actions using MsSql commands.
    /// </summary>
    internal class SqlServerProcedures : DbProcedures
    {
        public SqlServerProcedures(DbContext context) 
            : base(context) 
        {
            if (!(_context.Database.Connection is System.Data.SqlClient.SqlConnection))
                throw new Exception("Database connection is not a SqlConnection");
        }

        /// <summary>
        /// Creates a new table based on the file provided.
        /// </summary>
        /// <param name="file">CSV file.</param>
        /// <param name="dbContext">Reference to thte database.</param>
        internal override void AddTableFromFile(string file)
        {
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

            _context.Database.ExecuteSqlCommand(
                "CREATE TABLE " + Path.GetFileNameWithoutExtension(file) + " (" + columnsBuilder.ToString() + ")"
                );

            LoggerFactory.Log("Table " + Path.GetFileNameWithoutExtension(file) + " added to the database.");
        }

        internal override void PopulateTableFromFile(string file)
        {
            throw new NotImplementedException();
        }

        public override void SumTables(string attTable, string usTable, string calcTable)
        {
            throw new NotImplementedException();
        }

        public override void AvgTables(string attTable, string usTable, string calcTable)
        {
            throw new NotImplementedException();
        }

        public override void CreateDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }
    }
}
