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
    public class TableHub : UserHub
    {
        private static IHubContext _context;

        static TableHub() 
        {
            _context = GlobalHost.ConnectionManager.GetHubContext<TableHub>();     
        }

        /// <summary>
        /// Notifies the user who uploaded the table that there is a new table.
        /// </summary>
        /// <param name="tableIndex"></param>
        public static void NotifyNewTable(TableIndex tableIndex)
        {
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
                    var client = _context.Clients.Client(connectionId);
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
                    _context.Clients.Client(connectionId).updateTable(tableIndex);
                }
            }
        }

        public static void NotifyTableRemoved(TableIndex tableIndex)
        {
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
                    _context.Clients.Client(connectionId).removeTable(tableIndex);
                }
            }
        }
    }
}