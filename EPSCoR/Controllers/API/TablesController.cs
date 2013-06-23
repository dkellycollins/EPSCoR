using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Serialization;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Basic;
using Newtonsoft.Json;

namespace EPSCoR.Controllers.API
{
    public class TablesController : ApiController
    {
        JsonSerializer _serializer;
        JsonWriter _writer;

        public TablesController()
        {
            _serializer = new JsonSerializer();
        }

        // GET api/tables
        [AcceptVerbs("GET", "HEAD")]
        public IEnumerable<TableIndex> Get(string user)
        {
            if (string.IsNullOrEmpty(user))
                return null;

            using (IModelRepository<TableIndex> repo = RepositoryFactory.GetModelRepository<TableIndex>())
            {
                var tables = repo.GetAll().Where((t) =>
                    t.UploadedByUser == user);
                return tables;
            }
        }

        // GET api/tables/5
        [AcceptVerbs("GET", "HEAD")]
        public DataTable Get(string table, string user)
        {
            if (string.IsNullOrEmpty(table)
                || string.IsNullOrEmpty(user))
                return null;

            using (ITableRepository repo = RepositoryFactory.GetTableRepository(user))
            {
                var dataTable = repo.Read(table);
                dataTable.TableName = table;
                return dataTable;
            }
        }

        // POST api/tables
        [AcceptVerbs("POST")]
        public void Post([FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // PUT api/tables/5
        [AcceptVerbs("PUT")]
        public void Put(int id, [FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // DELETE api/tables/5
        [AcceptVerbs("DELETE")]
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
