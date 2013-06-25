using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Basic;
using EPSCoR.Repositories.Factory;

namespace EPSCoR.Controllers.API
{
    public class FileController : ApiController
    {

        // GET api/file
        [AcceptVerbs("GET", "HEAD")]
        public IEnumerable<string> Get(string user, string directory)
        {
            /*
            if(!validateUser(user))
                return new HttpNotFoundResult();

            IFileAccessor files;
            switch (directory.ToLower())
            {
                case "conversion":
                    files = RepositoryFactory.GetConvertionFileAccessor(user);
                    break;
                case "archive":
                    files = RepositoryFactory.GetArchiveFileAccessor(user);
                    break;
                default:
                    return new HttpNotFoundResult();
            }

            return files.GetFiles();
             */

            return new List<string>();
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

        private bool validateUser(string userName)
        {
            using(IModelRepository<UserProfile> userProfileRepo = RepositoryFactory.GetModelRepository<UserProfile>())
            {
                UserProfile profile = userProfileRepo.GetAll().Where((p) => p.UserName == userName).FirstOrDefault();
                return profile != null;
            }
        }
    }
}
