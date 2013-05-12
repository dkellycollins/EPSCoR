using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using EPSCoR.Models;

namespace EPSCoR.Repositories
{
    /// <summary>
    /// This repo handles create entirely new tables in the database.
    /// </summary>
    public class DynamicTableRepo : IRepository<DynamicTable>
    {
        private const string CREATE_TABLE = @"CREATE TABLE @table_name (@column_name @columnType)";
        private const string DROP_TABLE = @"DROP TABLE @table_name";
        private const string UPDATE_TABLE = @"UPDATE @table_name SET {1}={2} WHERE {3}={4}";
        private const string SELECT_TABLE = @"SELECT * FROM @table_name";


        private DbContext _context;

        public DynamicTableRepo()
        {
            _context = new DefaultContext();
        }

        public DynamicTableRepo(DbContext context) 
        {
            _context = context;
        }
        
        /// <summary>
        /// Gets a table from the database.
        /// </summary>
        /// <param name="entityID"></param>
        /// <returns></returns>
        public DynamicTable Get(int entityID)
        {
            var table = _context.Database.SqlQuery(SELECT_TABLE, entityID);
            //Create dynamic table.
        }

        /// <summary>
        /// Gets a table from the database.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DynamicTable Get(string tableName)
        {
            var table = _context.Database.SqlQuery(SELECT_TABLE, entityID);
            //convert in DynamicTable.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IQueryable<DynamicTable> GetAll()
        {
 	        throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new table in the database.
        /// </summary>
        /// <param name="itemToCreate"></param>
        public void Create(DynamicTable itemToCreate)
        {
 	        _context.Database.ExecuteSqlCommand(CREATE_TABLE, 
        }

        public void Update(DynamicTable itemToUpdate)
        {
 	        _context.Database.ExecuteSqlCommand(UPDATE_TABLE, 
        }

        public void Remove(int entityID)
        {
 	        _context.Database.ExecuteSqlCommand(DROP_TABLE, entityID); 
        }

        public void Remove(string tableName)
        {
            _context.Database.ExecuteSqlCommand(DROP_TABLE, tableName);
        }

        public void Dispose()
        {
 	        _context.Dispose();
        }
}