using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Factory;
using Microsoft.AspNet.SignalR;

namespace EPSCoR.Hubs
{
    /// <summary>
    /// Keeps track of users currently connected.
    /// </summary>
    public class UserHub : Hub
    {
        /// <summary>
        /// Adds connectionId to user.
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            using (IModelRepository<UserConnection> connectionRepo = RepositoryFactory.GetModelRepository<UserConnection>())
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
                else
                {
                    connectionRepo.Update(connection);
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

            using (IModelRepository<UserConnection> connectionRepo = RepositoryFactory.GetModelRepository<UserConnection>())
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
            using (IModelRepository<UserProfile> userRepo = RepositoryFactory.GetModelRepository<UserProfile>())
            {
                return userRepo.GetAll().Where((u) => u.UserName == userName).FirstOrDefault();
            }
        }

        protected static IEnumerable<UserConnection> GetConnectionsForUser(string userName)
        {
            using (IModelRepository<UserConnection> connectionRepo = RepositoryFactory.GetModelRepository<UserConnection>())
            {
                return connectionRepo.GetAll().Where((cid) => cid.User == userName).ToList();
            }
        }
    }
}