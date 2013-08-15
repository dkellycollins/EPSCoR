using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPSCoR.Web.Database.Context;
using EPSCoR.Web.Database.Models;

namespace EPSCoR.Web.Database
{
    public class DbWatcher
    {
        public delegate void ModelEventHandler(IModel model);

        /// <summary>
        /// Raised when a model has been created in the database.
        /// </summary>
        public static event ModelEventHandler ModelCreated = delegate { };

        /// <summary>
        /// Raised when a model has been updated in the database.
        /// </summary>
        public static event ModelEventHandler ModelUpdated = delegate { };

        /// <summary>
        /// Raised when a model has been removed from the datbase.
        /// </summary>
        public static event ModelEventHandler ModelRemoved = delegate { };

        private System.Timers.Timer _timer;
        private int _lastId;

        public DbWatcher()
        {
            _timer = new System.Timers.Timer()
            {
                AutoReset = true,
                Interval = 10000, // 10 seconds.
            };

            _timer.Elapsed += checkTable;
        }

        public void Start()
        {
            using (ModelDbContext context = DbContextFactory.GetModelDbContext())
            {
                DbEvent lastEvent = context.GetAllModels<DbEvent>().LastOrDefault();
                _lastId = (lastEvent == null) ? 0 : lastEvent.ID;
            }

            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void checkTable(object sender, System.Timers.ElapsedEventArgs args)
        {
            using (ModelDbContext context = DbContextFactory.GetModelDbContext())
            {
                IEnumerable<DbEvent> events = context.GetAllModels<DbEvent>().Where((e) => e.ID > _lastId);
                foreach (DbEvent e in events)
                {
                    IModel model = null;
                    switch (e.Table)
                    {
                        case "TableIndexes":
                            model = context.GetModel<TableIndex>(e.EntryID);
                            break;
                        default:
                            throw new Exception("Unsupported model type");
                    }
                    switch ((Models.Action)e.ActionCode)
                    {
                        case Models.Action.Created:
                            ModelCreated(model);
                            break;
                        case Models.Action.Deleted:
                            ModelRemoved(model);
                            break;
                        case Models.Action.Updated:
                            ModelUpdated(model);
                            break;
                    }

                    _lastId = e.ID;
                }
            }
        }
    }
}
