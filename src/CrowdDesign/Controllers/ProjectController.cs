using System.Linq;
using System.Net;
using System.Web.Mvc;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces;
using CrowdDesign.Infrastructure.SQLServer.Repositories;

namespace CrowdDesign.UI.Web.Controllers
{
    [Authorize]
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
        public ActionResult GetProjects()
        {
            if (System.Web.HttpContext.Current.Session["userIsAdmin"] != null)
                ViewBag.IsUserAdmin = (bool)System.Web.HttpContext.Current.Session["userIsAdmin"];
            else
                return RedirectToAction("Index", "Security");

            return View("ManageProjects", _repository.GetProjects());
        }

        [HttpPost]
        public ActionResult CreateProject()
        {
            if (ModelState.IsValid)
            {
                int projectId = _repository.CreateProject(new Project { Name = "New Project" });

                if (projectId > 0)
                    return RedirectToAction("GetProjects");
            }

            return View("Error");
        }
        
        public ActionResult EditProject(int? projectId)
        {
            if (System.Web.HttpContext.Current.Session["userId"] != null)
            {
                ViewBag.UserId = (int)System.Web.HttpContext.Current.Session["userId"];
                ViewBag.IsUserAdmin = (bool)System.Web.HttpContext.Current.Session["userIsAdmin"];

                if (projectId == null)
                    return View("Error");

                Project project = _repository.GetProjects(projectId.Value).SingleOrDefault();

                if (project == null)
                    return View("Error");

                return View("EditProject", project);
            }

            return RedirectToAction("Index", "Security");
        }

        [HttpPost]
        public ActionResult UpdateProject(Project project)
        {
            if (project == null || !ModelState.IsValid)
                return View("Error");

            _repository.UpdateProject(project);

            return RedirectToAction("EditProject", new { ProjectId = project.Id });
        }

        [HttpPost]
        public ActionResult DeleteProject(int? projectid)
        {
            if (projectid == null || !ModelState.IsValid)
                return View("Error");

            _repository.DeleteProject(projectid.Value);

            return RedirectToAction("GetProjects");
        }
        #endregion
    }
}
