using System;
using System.Data;
using System.Threading.Tasks;

namespace EPSCoR.Web.App.Repositories
{
    public interface IAsyncTableRepository : IDisposable
    {
        /// <summary>
        /// Creates a new table in the database using the given table.
        /// </summary>
        /// <param name="table"></param>
        Task CreateAsync(DataTable table);

        /// <summary>
        /// Reads the entire table into a DataTable object. Note that if the table is large this might throw an OutOfMemoryException.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        Task<DataTable> ReadAsync(string tableName);

        /// <summary>
        /// Reads a portion of the table into a DatabTable object.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="lowerLimit"></param>
        /// <param name="upperLimit"></param>
        /// <returns></returns>
        Task<DataTable> ReadAsync(string tableName, int lowerLimit, int upperLimit);

        /// <summary>
        /// Returns how many rows are in the table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        Task<int> CountAsync(string tableName);

        /// <summary>
        /// Updates the table using the given table.
        /// </summary>
        /// <param name="table"></param>
        Task UpdateAsync(DataTable table);

        /// <summary>
        /// Drops the entire table.
        /// </summary>
        /// <param name="tableName"></param>
        Task DropAsync(string tableName);
    }
}