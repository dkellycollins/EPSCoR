using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPSCoR.Web.App.Repositories;
using EPSCoR.Web.App.Repositories.Factory;
using EPSCoR.Web.Database.Models;
using Microsoft.AspNet.SignalR;

namespace EPSCoR.Web.App.Hubs
{
    /// <summary>
    /// Keeps track of users currently connected.
    /// </summary>
    public class UserHub : Hub
    {
        protected IRepositoryFactory _repoFactory;

        public UserHub()
        {
            _repoFactory = new RepositoryFactory();
        }

        public UserHub(IRepositoryFactory factory)
        {
            _repoFactory = factory;
        }

        /// <summary>
        /// Adds connectionId to user.
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            using (IModelRepository<UserConnection> connectionRepo = _repoFactory.GetModelRepository<UserConnection>())
            {
                UserConnection connection = connectionRepo.GetAll().Where((cid) => cid.ConnectionId == connectionId).FirstOrDefault();
                if (connection == null)
                {
                    connectionRepo.Create(new UserConnection()
                    {
                        User = userName,
                        ConnectionId = connectionId
                    });
                }
            }

            return base.OnConnected();
        }

        /// <summary>
        /// Removes connectionId from user.
        /// </summary>
        /// <returns></returns>
        public override Task OnDisconnected()
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            using (IModelRepository<UserConnection> connectionRepo = _repoFactory.GetModelRepository<UserConnection>())
            {
                UserConnection connection = connectionRepo.GetAll().Where((cid) => cid.User == userName && cid.ConnectionId == connectionId).FirstOrDefault();
                if (connection != null)
                {
                    connectionRepo.Remove(connection.ID);
                }
            }

            return base.OnDisconnected();
        }

        protected static UserProfile GetUserByUserName(string userName)
        {
            IRepositoryFactory repoFactory = new RepositoryFactory();
            using (IModelRepository<UserProfile> userRepo = repoFactory.GetModelRepository<UserProfile>())
            {
                return userRepo.GetAll().Where((u) => u.UserName == userName).FirstOrDefault();
            }
        }

        protected static IEnumerable<UserConnection> GetConnectionsForUser(string userName)
        {
            IRepositoryFactory repoFactory = new RepositoryFactory();
            using (IModelRepository<UserConnection> connectionRepo = repoFactory.GetModelRepository<UserConnection>())
            {
                return connectionRepo.GetAll().Where((cid) => cid.User == userName).ToList();
            }
        }
    }
}