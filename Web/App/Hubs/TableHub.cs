using System.Collections.Generic;
using System.Linq;
using BootstrapSupport;
using EPSCoR.Web.App.Repositories;
using EPSCoR.Web.App.Repositories.Factory;
using EPSCoR.Web.Database.Models;
using Microsoft.AspNet.SignalR;

namespace EPSCoR.Web.App.Hubs
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

        public TableHub()
            :base()
        {}

        public TableHub(IRepositoryFactory factory)
            :base(factory)
        {}

        /// <summary>
        /// Notifies the user who uploaded the table that there is a new table.
        /// </summary>
        /// <param name="tableIndex"></param>
        public static void NotifyNewTable(TableIndex tableIndex)
        {
            IEnumerable<UserConnection> connections = GetConnectionsForUser(tableIndex.UploadedByUser);
            foreach (UserConnection connection in connections)
            {
                var client = _context.Clients.Client(connection.ConnectionId);
                client.addTable(tableIndex);
            }
        }

        /// <summary>
        /// Notifies the user who uploaded the table that the table has been updated.
        /// </summary>
        /// <param name="tableIndex"></param>
        public static void NotifyTableUpdated(TableIndex tableIndex)
        {
            IEnumerable<UserConnection> connections = GetConnectionsForUser(tableIndex.UploadedByUser);
            foreach (UserConnection connection in connections)
            {
                _context.Clients.Client(connection.ConnectionId).updateTable(tableIndex);
            }
        }

        /// <summary>
        /// Notifies the user who uploaded the table that it has been removed.
        /// </summary>
        /// <param name="tableIndex"></param>
        public static void NotifyTableRemoved(TableIndex tableIndex)
        {
            IEnumerable<UserConnection> connections = GetConnectionsForUser(tableIndex.UploadedByUser);
            foreach (UserConnection connection in connections)
            {
                _context.Clients.Client(connection.ConnectionId).removeTable(tableIndex);
            }
        }

        /// <summary>
        /// Removes the given table.
        /// </summary>
        /// <param name="tableName">Name of the table to remove.</param>
        public async void RemoveTable(string tableName)
        {
            string userName = Context.User.Identity.Name;

            //Remove index.
            using (IModelRepository<TableIndex> tableIndexRepo = _repoFactory.GetModelRepository<TableIndex>())
            {
                TableIndex index = tableIndexRepo.GetAll().Where((i) => i.Name == tableName && i.UploadedByUser == userName).FirstOrDefault();
                if (index == null)
                {
                    AlertsHub.SendAlertToUser("Could not delete table " + tableName, userName, "", Alerts.ERROR);
                    return;
                }
                tableIndexRepo.Remove(index.ID);
            }

            //Drop table.
            using (IAsyncTableRepository tableRepo = _repoFactory.GetAsyncTableRepository(userName))
            {
                await tableRepo.DropAsync(tableName);
            }

            //Delete files.
            IAsyncFileAccessor fileAccessor = _repoFactory.GetAsyncFileAccessor(userName);
            await fileAccessor.DeleteFilesAsync(FileDirectory.Conversion, tableName);
            await fileAccessor.DeleteFilesAsync(FileDirectory.Archive, tableName);
            await fileAccessor.DeleteFilesAsync(FileDirectory.Upload, tableName);

            AlertsHub.SendAlertToUser(tableName + " has beeen deleted", userName);
        }

        /// <summary>
        /// Creates a new calc table.
        /// </summary>
        /// <param name="attTable">Attribute table to use.</param>
        /// <param name="usTable">Upstream table to use.</param>
        /// <param name="calcType">Type of calculation to perform.</param>
        public async void SubmitCalcTable(string attTable, string usTable, string calcType)
        {
            string userName = Context.User.Identity.Name;

            AlertsHub.SendAlertToUser("Your request has been submitted.", userName);

            //Validate input.
            using (IModelRepository<TableIndex> tableIndexRepo = _repoFactory.GetModelRepository<TableIndex>())
            {
                TableIndex attTableIndex = tableIndexRepo.GetAll().Where((i) => i.Name == attTable && i.UploadedByUser == userName).FirstOrDefault();
                TableIndex usTableIndex = tableIndexRepo.GetAll().Where((i) => i.Name == usTable && i.UploadedByUser == userName).FirstOrDefault();
                if (attTableIndex == null)
                {
                    AlertsHub.SendAlertToUser(attTable + " is not a valid table.", userName, "", Alerts.ERROR);
                    return;
                }
                if (usTableIndex == null)
                {
                    AlertsHub.SendAlertToUser(usTable + " is not a vlaid table.", userName, "", Alerts.ERROR);
                    return;
                }
            }

            using (IAsyncDatabaseCalc dbCalc = _repoFactory.GetAsyncDatabaseCalc(userName))
            {
                CalcResult result;
                switch (calcType)
                {
                    case "SUM":
                        result = await dbCalc.SumTablesAsync(attTable, usTable);
                        break;
                    case "AVG":
                        result = await dbCalc.AvgTablesAsync(attTable, usTable);
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