using System.Linq;
using System.Web.Mvc;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces.Repositories;
using CrowdDesign.Infrastructure.SQLServer.Contexts;
using CrowdDesign.Infrastructure.SQLServer.Repositories;

namespace CrowdDesign.UI.Web.Controllers
{
    [Authorize]
    public class ProjectController : BaseController<IBaseRepository<Project, int>, Project, int>
    {
        #region Constructors
        public ProjectController()
            : base(new ProjectRepository(new DatabaseContext()))
        { }
        #endregion

        #region Methods
        public ActionResult GetProjects()
        {
            if (System.Web.HttpContext.Current.Session["userIsAdmin"] != null)
                ViewBag.IsUserAdmin = (bool)System.Web.HttpContext.Current.Session["userIsAdmin"];
            else
                return RedirectToAction("Index", "Security");

            return View("ManageProjects", Repository.Get());
        }

        [HttpPost]
        public ActionResult CreateProject()
        {
            if (ModelState.IsValid)
            {
                int projectId = Repository.Create(new Project { Name = "New Project" });

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

                Project project = Repository.Get(projectId.Value).SingleOrDefault();

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

            Repository.Update(project);

            return RedirectToAction("EditProject", new { ProjectId = project.Id });
        }

        [HttpPost]
        public ActionResult DeleteProject(int? projectid)
        {
            if (projectid == null || !ModelState.IsValid)
                return View("Error");

            Repository.Delete(projectid.Value);

            return RedirectToAction("GetProjects");
        }
        #endregion
    }
}
