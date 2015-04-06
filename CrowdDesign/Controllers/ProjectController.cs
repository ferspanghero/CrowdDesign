using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces;
using CrowdDesign.Infrastructure.SQLServer;
using System.Collections;
using System.Web.Mvc;

namespace CrowdDesign.UI.Web.Controllers
{
    public class ProjectController : Controller
    {
        #region Constructors
        public ProjectController()
        {
            _repository = new ProjectRepository();
        }
        #endregion

        #region Fields
        private readonly IProjectRepository _repository;
        #endregion

        #region Methods
        public ActionResult Index()
        {
            return View("IndexProject", _repository.GetProjects());
        }

        [HttpPost]
        public ActionResult CreateProject()
        {
            if (ModelState.IsValid)
                _repository.CreateProject(new Project { Name = "New Project" });

            return RedirectToAction("Index");
        }

        public ActionResult EditProjectDetails(int? projectId)
        {
            if (projectId == null)
                return View("Error");

            Project project = _repository.GetProject(projectId.Value);

            if (project == null)
                return HttpNotFound();

            return View("EditProject", project);
        }

        [HttpPost]
        public ActionResult EditProjectName(Project project)
        {
            if (project == null)
                return View("Error");

            if (ModelState.IsValid)
                _repository.UpdateProject(project);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteProject(int? projectid)
        {
            if (projectid == null)
                return View("Error");

            if (ModelState.IsValid)
                _repository.DeleteProject(projectid.Value);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult CreateCategory(int? projectId)
        {
            string categoryName = Request["categoryName"];

            if (string.IsNullOrEmpty(categoryName) || projectId == null)
                return View("Error");

            if (ModelState.IsValid)
                _repository.CreateCategory(projectId.Value, categoryName);

            return RedirectToAction("EditProjectDetails", new { projectId });
        }

        [HttpPost]
        public ActionResult CreateUser(int? projectId)
        {
            string userName = Request["userName"];

            if (string.IsNullOrEmpty(userName) || projectId == null)
                return View("Error");

            if (ModelState.IsValid)
                _repository.CreateUser(projectId.Value, userName);

            return RedirectToAction("EditProjectDetails", new { projectId });
        }
        #endregion
    }
}
