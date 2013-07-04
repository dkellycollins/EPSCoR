﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace EPSCoR.Repositories
{
    public interface IAsyncTableRepository : IDisposable
    {
        /// <summary>
        /// Creates a new table in the database using table.
        /// </summary>
        /// <param name="table"></param>
        void CreateAsync(DataTable table);

        /// <summary>
        /// Reads the entire table into a Datatable object. Note that if the table is large this might throw an OutOfMemoryException.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        Task<DataTable> ReadAsync(string tableName);

        /// <summary>
        /// Reads a portion of the table into a Databtable object.
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
        /// Updates the table using table.
        /// </summary>
        /// <param name="table"></param>
        void UpdateAsync(DataTable table);

        /// <summary>
        /// Drops the entire table.
        /// </summary>
        /// <param name="tableName"></param>
        void DropAsync(string tableName);
    }
}