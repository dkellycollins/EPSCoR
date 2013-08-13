using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EPSCoR.Database.Models;
using EPSCoR.Filters;
using EPSCoR.Repositories;
using EPSCoR.Repositories.Factory;

namespace EPSCoR.Controllers
{
    public class ModelController<T> : Controller
        where T : class, IModel
    {
        protected IModelRepository<T> ModelRepo;

        protected ModelController()
        {
            ModelRepo = RepositoryFactory.GetModelRepository<T>();
        }

        protected ModelController(IModelRepository<T> modelRepo)
        {
            ModelRepo = modelRepo;
        }

        /// <summary>
        /// Gets all models of type T and displays the index view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult Index()
        {
            IEnumerable<T> models = ModelRepo.GetAll().ToList();

            if (Request.IsAjaxRequest())
                return PartialView(models);
            return View(models);
        }

        /// <summary>
        /// Attempts to find a model by the id. If one is found returns the details view. Otherwise returns HttpNotFound.
        /// </summary>
        /// <param name="id">Id of the model</param>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult Details(int id = 0)
        {
            T model = ModelRepo.Get(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            if (Request.IsAjaxRequest())
                return PartialView(model);
            return View(model);
        }

        /// <summary>
        /// Attemps to create a default instance of the model and return the create view.
        /// Note that any subclasses should probably override this method as the default for classes is null.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual ActionResult Create()
        {
            T model = default(T);

            if (Request.IsAjaxRequest())
                return PartialView(model);
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Create(T model)
        {
            if (ModelState.IsValid)
            {
                ModelRepo.Create(model);
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
                return PartialView(model);
            return View(model);
        }

        [HttpGet]
        public virtual ActionResult Edit(int id = 0)
        {
            T model = ModelRepo.Get(id);

            if (model == null)
            {
                return HttpNotFound();
            }

            if (Request.IsAjaxRequest())
                return PartialView("Create", model);
            return View("Create", model);
        }

        [HttpPost]
        public virtual ActionResult Edit(T model)
        {
            if (ModelState.IsValid)
            {
                ModelRepo.Update(model);
                return RedirectToAction("Index");
            }

            if (Request.IsAjaxRequest())
                return PartialView("Create", model);
            return View("Create", model);
        }

        public virtual ActionResult Delete(int id)
        {
            ModelRepo.Remove(id);

            return RedirectToAction("Index");
        }
    }

    [AddUserWhenAuthorized(Roles = "Admin")]
    public class TableIndexController : ModelController<TableIndex>
    {
        private ITableRepository _tableRepo;

        public TableIndexController()
            : base()
        { }

        public TableIndexController(IModelRepository<TableIndex> repo)
            : base(repo)
        { }

        [HttpGet]
        public override ActionResult Create()
        {
            TableIndex tableIndex = new TableIndex();

            if (Request.IsAjaxRequest())
                return PartialView(tableIndex);
            return View(tableIndex);
        }

        public override ActionResult Delete(int id)
        {
            string tableName;
            string userName;

            //Remove index.
            TableIndex index = ModelRepo.Get(id);

            if (index == null)
            {
                return HttpNotFound();
            }

            tableName = index.Name;
            userName = index.UploadedByUser;
            ModelRepo.Remove(index.ID);

            //Drop table.
            using (ITableRepository tableRepo = RepositoryFactory.GetTableRepository(userName))
            {
                tableRepo.Drop(tableName);
            }

            //Delete files.
            IFileAccessor fileAccessor = RepositoryFactory.GetFileAccessor(userName);
            fileAccessor.DeleteFiles(FileDirectory.Conversion, tableName);
            fileAccessor.DeleteFiles(FileDirectory.Archive, tableName);
            fileAccessor.DeleteFiles(FileDirectory.Upload, tableName);

            return RedirectToAction("Index");
        }
    }

    [AddUserWhenAuthorized(Roles = "Admin")]
    public class UserProfileController : ModelController<UserProfile>
    {
        private IModelRepository<TableIndex> _tableIndexRepo;

        public UserProfileController()
            : base()
        {
            _tableIndexRepo = RepositoryFactory.GetModelRepository<TableIndex>();
        }

        public UserProfileController(IModelRepository<UserProfile> userProfileRepo, IModelRepository<TableIndex> tableIndexRepo)
            : base(userProfileRepo)
        {
            _tableIndexRepo = tableIndexRepo;
        }

        [HttpGet]
        public override ActionResult Create()
        {
            UserProfile userProfile = new UserProfile();

            if (Request.IsAjaxRequest())
                return PartialView(userProfile);
            return View(userProfile);
        }

        public override ActionResult Delete(int id)
        {
            UserProfile userProfile = ModelRepo.Get(id);
            if (userProfile == null)
            {
                return HttpNotFound();
            }

            ModelRepo.Remove(userProfile.ID);

            IEnumerable<TableIndex> userTables = _tableIndexRepo.GetAll().Where((t) => t.UploadedByUser == userProfile.UserName).ToList();

            foreach (TableIndex table in userTables)
            {
                //Remove index.
                _tableIndexRepo.Remove(table.ID);

                //Drop table.
                using (ITableRepository tableRepo = RepositoryFactory.GetTableRepository(userProfile.UserName))
                {
                    tableRepo.Drop(table.Name);
                }

                //Delete files.
                IFileAccessor fileAccessor = RepositoryFactory.GetFileAccessor(userProfile.UserName);
                fileAccessor.DeleteFiles(FileDirectory.Conversion, table.Name);
                fileAccessor.DeleteFiles(FileDirectory.Archive, table.Name);
                fileAccessor.DeleteFiles(FileDirectory.Upload, table.Name);
            }

            return RedirectToAction("Index");
        }
    }

    public class LogController : ModelController<LogEntry>
    {
        private IModelRepository<LogEntry> _logRepo;

        public LogController()
        {
            _logRepo = RepositoryFactory.GetModelRepository<LogEntry>();
        }

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