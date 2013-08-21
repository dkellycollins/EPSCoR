using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPSCoR.Web.App.Filters;
using EPSCoR.Web.App.Repositories;
using EPSCoR.Web.App.Repositories.Factory;
using EPSCoR.Web.Database.Models;

namespace EPSCoR.Web.App.Controllers
{
    public class ModelController<T> : Controller
        where T : Model, new() 
    {
        protected IRepositoryFactory _repoFactory;

        protected ModelController()
        {
            _repoFactory = new RepositoryFactory();
        }

        protected ModelController(IRepositoryFactory factory)
        {
            _repoFactory = factory;
        }

        /// <summary>
        /// Gets all models of type T and displays the index view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [MultipleResponseFormats]
        public virtual ActionResult Index()
        {
            using (IModelRepository<T> repo = _repoFactory.GetModelRepository<T>())
            {
                return View(repo.GetAll());
            }
        }

        /// <summary>
        /// Attempts to find a model by the id. If one is found returns the details view. Otherwise returns HttpNotFound.
        /// </summary>
        /// <param name="id">Id of the model</param>
        /// <returns></returns>
        [HttpGet]
        [MultipleResponseFormats]
        public virtual ActionResult Details(int id = 0)
        {
            T model = null;
            using (IModelRepository<T> repo = _repoFactory.GetModelRepository<T>())
            {
                model = repo.Get(id);
            }

            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        /// <summary>
        /// Attemps to create a default instance of the model and return the create view.
        /// Note that any subclasses should probably override this method as the default for classes is null.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [MultipleResponseFormats(ResponseFormat.Ajax)]
        public virtual ActionResult Create()
        {
            T model = new T();

            return View(model);
        }

        [HttpPost]
        [MultipleResponseFormats(ResponseFormat.Ajax)]
        public virtual ActionResult Create(T model)
        {
            if (ModelState.IsValid)
            {
                using (IModelRepository<T> repo = _repoFactory.GetModelRepository<T>())
                {
                    repo.Create(model);
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        [MultipleResponseFormats(ResponseFormat.Ajax)]
        public virtual ActionResult Edit(int id = 0)
        {
            T model = null;
            using (IModelRepository<T> repo = _repoFactory.GetModelRepository<T>())
            {
                model = repo.Get(id);
            }

            if (model == null)
            {
                return HttpNotFound();
            }

            return View("Create", model);
        }

        [HttpPost]
        [MultipleResponseFormats(ResponseFormat.Ajax)]
        public virtual ActionResult Edit(T model)
        {
            if (ModelState.IsValid)
            {
                using (IModelRepository<T> repo = _repoFactory.GetModelRepository<T>())
                {
                    repo.Update(model);
                }
                return RedirectToAction("Index");
            }

            return View("Create", model);
        }

        [HttpGet]
        public virtual ActionResult Delete(int id)
        {
            using (IModelRepository<T> repo = _repoFactory.GetModelRepository<T>())
            {
                repo.Remove(id);
            }

            return RedirectToAction("Index");
        }
    }

    [AddUserWhenAuthorized(Roles = "Admin")]
    public class TableIndexController : ModelController<TableIndex>
    {
        public TableIndexController()
            : base()
        { }

        public TableIndexController(IRepositoryFactory factory)
            : base(factory)
        { }

        public override ActionResult Delete(int id)
        {
            string tableName;
            string userName;

            //Remove index.
            using (IModelRepository<TableIndex> repo = _repoFactory.GetModelRepository<TableIndex>())
            {
                TableIndex index = repo.Get(id);

                if (index == null)
                {
                    return HttpNotFound();
                }

                tableName = index.Name;
                userName = index.UploadedByUser;
                repo.Remove(index.ID);
            }

            //Drop table.
            using (ITableRepository tableRepo = _repoFactory.GetTableRepository(userName))
            {
                tableRepo.Drop(tableName);
            }

            //Delete files.
            IFileAccessor fileAccessor = _repoFactory.GetFileAccessor(userName);
            fileAccessor.DeleteFiles(FileDirectory.Conversion, tableName);
            fileAccessor.DeleteFiles(FileDirectory.Archive, tableName);
            fileAccessor.DeleteFiles(FileDirectory.Upload, tableName);

            return RedirectToAction("Index");
        }
    }

    [AddUserWhenAuthorized(Roles = "Admin")]
    public class UserProfileController : ModelController<UserProfile>
    {
        public UserProfileController()
            : base()
        { }

        public UserProfileController(IRepositoryFactory factory)
            : base(factory)
        { }

        public override ActionResult Delete(int id)
        {
            using (IModelRepository<UserProfile> userRepo = _repoFactory.GetModelRepository<UserProfile>())
            {
                UserProfile userProfile = userRepo.Get(id);
                if (userProfile == null)
                {
                    return HttpNotFound();
                }

                userRepo.Remove(userProfile.ID);

                using (IModelRepository<TableIndex> tableIndexRepo = _repoFactory.GetModelRepository<TableIndex>())
                {
                    IEnumerable<TableIndex> userTables = tableIndexRepo.Where((t) => t.UploadedByUser == userProfile.UserName);

                    foreach (TableIndex table in userTables)
                    {
                        //Remove index.
                        tableIndexRepo.Remove(table.ID);

                        //Drop table.
                        using (ITableRepository tableRepo = _repoFactory.GetTableRepository(userProfile.UserName))
                        {
                            tableRepo.Drop(table.Name);
                        }

                        //Delete files.
                        IFileAccessor fileAccessor = _repoFactory.GetFileAccessor(userProfile.UserName);
                        fileAccessor.DeleteFiles(FileDirectory.Conversion, table.Name);
                        fileAccessor.DeleteFiles(FileDirectory.Archive, table.Name);
                        fileAccessor.DeleteFiles(FileDirectory.Upload, table.Name);
                    }
                }
            }

            return RedirectToAction("Index");
        }
    }

    public class LogController : ModelController<LogEntry>
    {
        public LogController()
            :base ()
        { }

        public LogController(IRepositoryFactory factory)
            : base(factory)
        { }

        public override ActionResult Create()
        {
            return View("NotAuthorized");
        }

        public override ActionResult Create(LogEntry model)
        {
            return View("NotAuthorized");
        }

        public override ActionResult Delete(int id)
        {
            return View("NotAuthorized");
        }

        public override ActionResult Edit(LogEntry model)
        {
            return View("NotAuthorized");
        }

        public override ActionResult Edit(int id = 0)
        {
            return View("NotAuthorized");
        }


    }
}