using System;
using System.Data;

namespace EPSCoR.Web.App.Repositories
{
    /// <summary>
    /// Provides crud operation for working with an entire table.
    /// </summary>
    public interface ITableRepository : IDisposable
    {
        /// <summary>
        /// Creates a new table in the database using table.
        /// </summary>
        /// <param name="table"></param>
        void Create(DataTable table);
        
        /// <summary>
        /// Reads the entire table into a Datatable object. Note that if the table is large this might throw an OutOfMemoryException.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        DataTable Read(string tableName);
        
        /// <summary>
        /// Reads a portion of the table into a Databtable object.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="lowerLimit"></param>
        /// <param name="upperLimit"></param>
        /// <returns></returns>
        DataTable Read(string tableName, int lowerLimit, int upperLimit);

        /// <summary>
        /// Returns how many rows are in the table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        int Count(string tableName);

        /// <summary>
        /// Updates the table using table.
        /// </summary>
        /// <param name="table"></param>
        void Update(DataTable table);

        /// <summary>
        /// Drops the entire table.
        /// </summary>
        /// <param name="tableName"></param>
        void Drop(string tableName);
    }
}