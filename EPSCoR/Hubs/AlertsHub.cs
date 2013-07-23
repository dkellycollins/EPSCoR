using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BootstrapSupport;
using Microsoft.AspNet.SignalR;

namespace EPSCoR.Hubs
{
    /// <summary>
    /// Handles send alerts to users.
    /// </summary>
    public class AlertsHub : UserHub
    {
        private static IHubContext _context;

        /// <summary>
        /// Initializer.
        /// </summary>
        static AlertsHub()
        {
            _context = GlobalHost.ConnectionManager.GetHubContext<AlertsHub>();
        }

        /// <summary>
        /// Sends an alert to the given user. If the user cannot be found then nothign is sent.
        /// </summary>
        /// <param name="message">The body of the alert.</param>
        /// <param name="userName">Name of the user to send the message to.</param>
        /// <param name="header">Header of the alert.</param>
        /// <param name="alertType">The type of alert. This should be one of the alerts in BootstrapSupport.Alerts</param>
        public static void SendAlertToUser(string message, string userName, string header = "", string alertType = Alerts.INFORMATION)
        {
            User user;
            Users.TryGetValue(userName, out user);
            if (user != null)
            {
                IEnumerable<string> connectionIds;
                lock (user.ConnectionIds)
                {
                    connectionIds = user.ConnectionIds.ToList();
                }
                foreach (string connectionId in connectionIds)
                {
                    _context.Clients.Client(connectionId).newAlert(message, header, alertType);
                }
            }
        }

        /// <summary>
        /// Sends an alert to all users.
        /// </summary>
        /// <param name="message">The body of the alert.</param>
        /// <param name="header">Header of the alert.</param>
        /// <param name="alertType">The type of alert. This should be one of the alerts in BootstrapSupport.Alerts</param>
        public static void SendAlertToAll(string message, string header = "", string alertType = Alerts.INFORMATION)
        {
            _context.Clients.All.newAlert(message, header, alertType);
        }
    }
}