using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BootstrapSupport;
using EPSCoR.Database.Models;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Factory;
using Microsoft.AspNet.SignalR;
using WebMatrix.WebData;

namespace EPSCoR.Controllers
{
    [Authorize]
    public class NewController : Controller
    {
        public ActionResult Index()
        {
            using (IModelRepository<TableIndex> repo = RepositoryFactory.GetModelRepository<TableIndex>())
            {
                return View(
                    "~/Views/2.0/Index.cshtml",
                    "~/Views/2.0/Layout2.0.cshtml",
                    repo.GetAll()
                        .Where(index => index.UploadedByUser == WebSecurity.CurrentUserName)
                        .ToList()
                );
            }
        }

        public ActionResult CalcForm()
        {
            using (IModelRepository<TableIndex> repo = RepositoryFactory.GetModelRepository<TableIndex>())
            {
                var tables = repo.GetAll();

                var allTables = tables.Where(t => t.Processed && t.UploadedByUser == WebSecurity.CurrentUserName);
                var attTables = allTables.Where(t => t.Type == TableTypes.ATTRIBUTE);
                var usTables = allTables.Where(t => t.Type == TableTypes.UPSTREAM);

                ViewBag.AllTables = allTables.ToList();
                ViewBag.AttributeTables = attTables.ToList();
                ViewBag.UpstreamTables = usTables.ToList();

                return View(
                    "~/Views/Tables/CalcForm.cshtml",
                    "~/Views/2.0/Layout2.0.cshtml",
                    null);
            }
        }

        public ActionResult UploadForm()
        {
            return View(
                "~/Views/Files/Upload.cshtml",
                "~/Views/2.0/Layout2.0.cshtml",
                null);
        }

    }

    public class User {

        public string Name { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }

    [Microsoft.AspNet.SignalR.Authorize]
    public class TableHub : Hub
    {
        //Note that this should be moved to persistant storage.
        private static readonly ConcurrentDictionary<string, User> Users = new ConcurrentDictionary<string, User>();

        public override Task OnConnected() {

            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            var user = Users.GetOrAdd(userName, _ => new User {
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
        public void NewTable(TableIndex tableIndex)
        {
            User user = Users[tableIndex.UploadedByUser];
            if(user != null)
            {
                foreach(string connectionId in user.ConnectionIds)
                {
                    Clients.Client(connectionId).newTable(tableIndex);
                }
            }
        }

        /// <summary>
        /// Notifies the user who uploaded the table that the table has been updated.
        /// </summary>
        /// <param name="tableIndex"></param>
        public void UpdateTable(TableIndex tableIndex)
        {
            User user = Users[tableIndex.UploadedByUser];
            if(user != null)
            {
                foreach(string connectionId in user.ConnectionIds)
                {
                    Clients.Client(connectionId).updateTable(tableIndex);
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
        public void SendAlert(string message, string userName = null, string header = "", string alertType = Alerts.INFORMATION)
        {
            if(string.IsNullOrEmpty(userName))
            {
                Clients.All.newAlert(message, header, alertType);
            }
            else
            {
                User user = Users[userName];
                if(user != null)
                {
                    foreach(string connectionId in user.ConnectionIds)
                    {
                        Clients.Client(connectionId).newAlert(message, header, alertType);
                    }
                }
            }
        }
    }
}
