﻿using System.Collections.Generic;
using System.Linq;
using BootstrapSupport;
using EPSCoR.Web.Database.Models;
using Microsoft.AspNet.SignalR;

namespace EPSCoR.Web.App.Hubs
{
    /// <summary>
    /// Handles sending alerts to users.
    /// </summary>
    public class AlertsHub : UserHub
    {
        private static IHubContext _context;

        static AlertsHub()
        {
            _context = GlobalHost.ConnectionManager.GetHubContext<AlertsHub>();
        }

        /// <summary>
        /// Sends an alert to the given user. If the user cannot be found then nothing is sent.
        /// </summary>
        /// <param name="message">The body of the alert.</param>
        /// <param name="userName">Name of the user to send the message to.</param>
        /// <param name="header">Header of the alert.</param>
        /// <param name="alertType">The type of alert. This should be one of the alerts in BootstrapSupport.Alerts</param>
        public static void SendAlertToUser(string message, string userName, string header = "", string alertType = Alerts.INFORMATION)
        {
            UserProfile user = GetUserByUserName(userName);
            if (user != null)
            {
                IEnumerable<UserConnection> connections = GetConnectionsForUser(userName);
                foreach (UserConnection connection in connections)
                {
                    _context.Clients.Client(connection.ConnectionId).newAlert(message, header, alertType);
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