using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EPSCoR.Repositories
{
    public class DataTableRepo
    {
        private const string CREATE_TABLE = @"CREATE TABLE IF NOT EXIST @tableName";
        private const string CREATE_TABLE_STRING_COLUMN = @"@columnName, varchar(25)";
        private const string CREATE_TABLE_DOUBLE_COLUMN = @"@columnName, double";
        private const string LOAD_TABLE_DATA = "LOAD DATA LOCAL INFILE '@fileName' INTO TABLE @tableName FIELDS TERMINATED BY ',' OPTIONALLY ENCLOSED BY '\"' LINES TERMINATED BY '\n' IGNORE 1 LINES";
        private const string DROP_TABLE = @"DROP TABLE @tableName";
        private const string UPDATE_TABLE = @"UPDATE @table_name SET {1}={2} WHERE {3}={4}";
        private const string SELECT_TABLE = @"SELECT * FROM @tableName";
        private const string DELETE_ROW = string.Empty;
        private const string INSERT_ROW = string.Empty;

        private static SqlConnection sqlConnection
        {
            get
            {
                return new SqlConnection(DefaultContext.Instance.Database.Connection.ConnectionString);
            }
        }

        public static DataTable GetTable(string tableName)
        {
            DataTable table = new DataTable();
            string query = string.Format(SELECT_TABLE, tableName);

            SqlCommand cmd = new SqlCommand(SELECT_TABLE, sqlConnection);
            cmd.Parameters.Add("@Table", tableName);

            using (SqlDataAdapter adaptor = new SqlDataAdapter(cmd))
            {
                adaptor.Fill(table);
            }

            table.ColumnChanged += table_ColumnChanged;
            table.RowChanged += table_RowChanged;
            table.RowDeleted += table_RowDeleted;
            table.TableCleared += table_TableCleared;
            table.TableNewRow += table_TableNewRow;

            return table;
        }

        public static DataTable CreateTable(string tableName)
        {
            DefaultContext.Instance.Database.ExecuteSqlCommand(
                string.Format(CREATE_TABLE, tableName),
                null);

            return GetTable(tableName);
        }

        private static void table_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            DataTable table = (DataTable)sender;
        }

        private static void table_TableCleared(object sender, DataTableClearEventArgs e)
        {
            DataTable table = (DataTable)sender;
        }

        private static void table_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            DataTable table = (DataTable)sender;
        }

        private static void table_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            DataTable table = (DataTable)sender;
        }

        private static void table_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            DataTable table = (DataTable)sender;
        }

        private static void updateTableInDatabase(DataTable table)
        {
            SqlConnection conn = new SqlConnection(DefaultContext.Instance.Database.Connection.ConnectionString);
            SqlCommand selectCmd = new SqlCommand(SELECT_TABLE, conn);
            SqlCommand updateCmd = new SqlCommand(string.Format(UPDATE_TABLE), conn);
            SqlCommand insertCmd = new SqlCommand(string.Format(INSERT_ROW), conn);
            SqlCommand deleteCmd = new SqlCommand(string.Format(DELETE_ROW), conn);

            SqlDataAdapter adaptor = new SqlDataAdapter(selectCmd)
            {
                UpdateCommand = updateCmd,
                InsertCommand = insertCmd,
                DeleteCommand = deleteCmd,
            };

            adaptor.Update(table);

            adaptor.Dispose();
        }
    }
}