using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Basic;

namespace EPSCoR.Controllers.API
{
    public class FileController : ApiController
    {

        // GET api/file
        [AcceptVerbs("GET", "HEAD")]
        public IEnumerable<string> Get(string user)
        {
            if (string.IsNullOrEmpty(user))
                return new List<string>();

            IFileAccessor conversionFiles = BasicFileAccessor.GetConversionsAccessor(user);
            return conversionFiles.GetFiles();
        }

        // GET api/file/5
        [AcceptVerbs("GET", "HEAD")]
        public string Get(int id)
        {
            throw new NotImplementedException();
        }

        // POST api/file
        [AcceptVerbs("POST")]
        public void Post([FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // PUT api/file/5
        [AcceptVerbs("PUT")]
        public void Put(int id, [FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // DELETE api/file/5
        [AcceptVerbs("DELETE")]
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
