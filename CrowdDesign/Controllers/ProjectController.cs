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
        #region Project Listing
        public ActionResult Index()
        {
            return View("IndexProject", _repository.GetProjects());
        }

        [HttpPost]
        public ActionResult CreateProject()
        {
            if (ModelState.IsValid)
            {
                int projectId = _repository.CreateProject("New Project");

                if (projectId > 0)
                    return RedirectToAction("Index");
            }

            return View("Error");
        }
        #endregion

        #region Project Details
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

            return RedirectToAction("EditProjectDetails", new { ProjectId = project.Id });
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
            if (projectId == null)
                return View("Error");            

            if (ModelState.IsValid)
            {
                int categoryId = _repository.CreateCategory(projectId.Value, "New category");

                if (categoryId > 0)
                    return RedirectToAction("EditProjectDetails", new {projectId});
            }

            return View("Error");
        }

        [HttpPost]
        public ActionResult CreateSketch(int? categoryId)
        {
            if (categoryId == null)
                return View("Error");

            if (ModelState.IsValid)
            {
                int sketchId = _repository.CreateSketch(categoryId.Value);

                if (sketchId > 0)
                    return RedirectToAction("EditSketch", new {sketchId});
            }

            return View("Error");
        }
        #endregion

        #region Sketch
        public ActionResult EditSketch(int? sketchId)
        {
            if (sketchId == null)
                return View("Error");

            Sketch sketch = _repository.GetSketch(sketchId.Value);

            if (sketch == null)
                return HttpNotFound();

            return View("EditSketch", sketch);
        }

        [HttpPost]
        public ActionResult UpdateSketch(Sketch sketch, int? projectId)
        {
            if (sketch == null || projectId == null)
                return View("Error");

            if (ModelState.IsValid)
                _repository.UpdateSketch(sketch);

            return RedirectToAction("EditProjectDetails", new { ProjectId = projectId.Value });
        }
        #endregion
        #endregion
    }
}
