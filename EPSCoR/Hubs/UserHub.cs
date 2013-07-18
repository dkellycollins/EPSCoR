using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace EPSCoR.Hubs
{
    /// <summary>
    /// Keeps track of users currently connected.
    /// </summary>
    public class UserHub : Hub
    {
        public class User
        {
            public string Name { get; set; }
            public HashSet<string> ConnectionIds { get; set; }
        }

        //Note that this should be moved to persistant storage.
        protected static readonly ConcurrentDictionary<string, User> Users = new ConcurrentDictionary<string, User>();

        /// <summary>
        /// Adds connectionId to user.
        /// </summary>
        /// <returns></returns>
        public override Task OnConnected()
        {
            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            var user = Users.GetOrAdd(userName, _ => new User
            {
                Name = userName,
                ConnectionIds = new HashSet<string>()
            });

            lock (user.ConnectionIds)
            {
                user.ConnectionIds.Add(connectionId);
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

            User user;
            Users.TryGetValue(userName, out user);

            if (user != null)
            {
                lock (user.ConnectionIds)
                {
                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));

                    if (!user.ConnectionIds.Any())
                    {
                        User removedUser;
                        Users.TryRemove(userName, out removedUser);
                    }
                }
            }

            return base.OnDisconnected();
        }
    }
}