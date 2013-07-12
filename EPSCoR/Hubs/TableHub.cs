using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BootstrapSupport;
using EPSCoR.Database.Models;
using Microsoft.AspNet.SignalR;

namespace EPSCoR.Hubs
{
    public class User
    {

        public string Name { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }

    public class TableHub : Hub
    {
        //Note that this should be moved to persistant storage.
        private static readonly ConcurrentDictionary<string, User> Users = new ConcurrentDictionary<string, User>();

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

        /// <summary>
        /// Notifies the user who uploaded the table that there is a new table.
        /// </summary>
        /// <param name="tableIndex"></param>
        public static void NotifyNewTable(TableIndex tableIndex)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<TableHub>();

            User user;
            Users.TryGetValue(tableIndex.UploadedByUser, out user);
            if (user != null)
            {
                IEnumerable<string> connectionIds;
                lock (user.ConnectionIds)
                {
                    connectionIds = user.ConnectionIds;
                }
                foreach (string connectionId in connectionIds)
                {
                    var client = context.Clients.Client(connectionId);
                    client.addTable(tableIndex);
                }
            }
        }

        /// <summary>
        /// Notifies the user who uploaded the table that the table has been updated.
        /// </summary>
        /// <param name="tableIndex"></param>
        public static void NotifyTableUpdated(TableIndex tableIndex)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<TableHub>();

            User user;
            Users.TryGetValue(tableIndex.UploadedByUser, out user);
            if (user != null)
            {
                IEnumerable<string> connectionIds;
                lock (user.ConnectionIds)
                {
                    connectionIds = user.ConnectionIds;
                }
                foreach (string connectionId in connectionIds)
                {
                    context.Clients.Client(connectionId).updateTable(tableIndex);
                }
            }
        }

        public static void NotifyTableRemoved(TableIndex tableIndex)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<TableHub>();

            User user;
            Users.TryGetValue(tableIndex.UploadedByUser, out user);
            if (user != null)
            {
                IEnumerable<string> connectionIds;
                lock (user.ConnectionIds)
                {
                    connectionIds = user.ConnectionIds;
                }
                foreach (string connectionId in connectionIds)
                {
                    context.Clients.Client(connectionId).removeTable(tableIndex);
                }
            }
        }

        /// <summary>
        /// Sends an alert to clients. If no user name is given then will broadcast the message to all users.
        /// </summary>
        /// <param name="message">The body of the alert.</param>
        /// <param name="userName">Name of the user to send the message to.</param>
        /// <param name="header">Header of the alert.</param>
        /// <param name="alertType">The type of alert. This should be one of the alerts in BootstrapSupport.Alerts</param>
        public static void SendAlert(string message, string userName = null, string header = "", string alertType = Alerts.INFORMATION)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<TableHub>();

            if (string.IsNullOrEmpty(userName))
            {
                context.Clients.All.newAlert(message, header, alertType);
            }
            else
            {
                User user;
                Users.TryGetValue(userName, out user);
                if (user != null)
                {
                    IEnumerable<string> connectionIds;
                    lock (user.ConnectionIds)
                    {
                        connectionIds = user.ConnectionIds;
                    }
                    foreach (string connectionId in connectionIds)
                    {
                        context.Clients.Client(connectionId).newAlert(message, header, alertType);
                    }
                }
            }
        }
    }
}