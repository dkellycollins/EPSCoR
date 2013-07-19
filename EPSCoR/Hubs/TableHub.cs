using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BootstrapSupport;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Factory;
using Microsoft.AspNet.SignalR;

namespace EPSCoR.Hubs
{
    /// <summary>
    /// 
    /// </summary>
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

        /// <summary>
        /// Notifies the user who uploaded the table that it has been removed.
        /// </summary>
        /// <param name="tableIndex"></param>
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

        /// <summary>
        /// Removes the given table.
        /// </summary>
        /// <param name="tableName">Name of the table to remove.</param>
        public void RemoveTable(string tableName)
        {
            string userName = Context.User.Identity.Name;
            using (ITableRepository repo = RepositoryFactory.GetTableRepository(userName))
            {
                repo.Drop(tableName);
            }
            IFileAccessor fileAccessor = RepositoryFactory.GetFileAccessor(userName);
            fileAccessor.CurrentDirectory = FileDirectory.Conversion;
            fileAccessor.DeleteFiles(tableName);

            fileAccessor.CurrentDirectory = FileDirectory.Archive;
            fileAccessor.DeleteFiles(tableName);

            AlertsHub.SendAlertToUser(tableName + " has beeen deleted", userName);
        }

        /// <summary>
        /// Creates a new calc table.
        /// </summary>
        /// <param name="attTable">Attribute table to use.</param>
        /// <param name="usTable">Upstream table to use.</param>
        /// <param name="calcType">Type of calculation to perform.</param>
        public void SubmitCalcTable(string attTable, string usTable, string calcType)
        {
            string userName = Context.User.Identity.Name;

            AlertsHub.SendAlertToUser("Your request has been submitted.", userName);

            using (IDatabaseCalc dbCalc = RepositoryFactory.GetDatabaseCalc(userName))
            {
                CalcResult result;
                switch (calcType)
                {
                    case "Sum":
                        result = dbCalc.SumTables(attTable, usTable);
                        break;
                    case "Avg":
                        result = dbCalc.AvgTables(attTable, usTable);
                        break;
                    default:
                        result = CalcResult.Unknown;
                        break;
                }

                switch (result)
                {
                    case CalcResult.Error:
                        AlertsHub.SendAlertToUser("An error occured while generating the table.", userName, "", Alerts.ERROR);
                        break;
                    case CalcResult.Success:
                        AlertsHub.SendAlertToUser("Calc table successfully generated.", userName, "", Alerts.SUCCESS);
                        break;
                    case CalcResult.TableAlreadyExists:
                        AlertsHub.SendAlertToUser("Calc table already exists. Please delete existing table before generating a new one.", userName, "", Alerts.ERROR);
                        break;
                    case CalcResult.Unknown:
                        AlertsHub.SendAlertToUser("An unknown error occured.", userName, "", Alerts.ERROR);
                        break;
                }
            }
        }
    }
}