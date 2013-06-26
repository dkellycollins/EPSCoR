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
using System.IO;
using System.Net.Http.Headers;
using EPSCoR.ViewModels;
using WebMatrix.WebData;

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
        public HttpResponseMessage Get(string fileName)
        {
            IFileAccessor convertionFileAccessor = RepositoryFactory.GetConvertionFileAccessor(WebSecurity.CurrentUserName);
            IFileAccessor archiveFileAccessor = RepositoryFactory.GetArchiveFileAccessor(WebSecurity.CurrentUserName);
            HttpResponseMessage response = new HttpResponseMessage();

            if(convertionFileAccessor.FileExist(fileName))
            {
                Stream fileStream = convertionFileAccessor.OpenFile(fileName);
                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StreamContent(fileStream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            }
            else
            {
                response.StatusCode = HttpStatusCode.NotFound;
            }

            return response;
        }

        // POST api/file
        [AcceptVerbs("POST")]
        public HttpResponseMessage Post(FileUpload fileUpload)
        {
            IFileAccessor uploadFileAccessor = RepositoryFactory.GetUploadFileAccessor(WebSecurity.CurrentUserName);
            bool result = uploadFileAccessor.SaveFiles(FileStreamWrapper.FromFileUpload(fileUpload));

            HttpResponseMessage response = new HttpResponseMessage();
            if (result)
            {
                response.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return response;
        }

        // PUT api/file/5
        [AcceptVerbs("PUT")]
        public HttpResponseMessage Put(int id, [FromBody]string value)
        {
            throw new NotImplementedException();
        }

        // DELETE api/file/5
        [AcceptVerbs("DELETE")]
        public HttpResponseMessage Delete(int id)
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
