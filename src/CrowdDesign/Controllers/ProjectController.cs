using System.Web.Mvc;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces;
using CrowdDesign.Infrastructure.SQLServer;
using CrowdDesign.Infrastructure.SQLServer.Repositories;
using CrowdDesign.UI.Web.Models;
using CrowdDesign.UI.Web.Models.Project;

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
        #region Project Management
        [Authorize]
        public ActionResult GetProjects()
        {
            ViewBag.IsUserAdmin = (bool)System.Web.HttpContext.Current.Session["userIsAdmin"];

            return View("ManageProjects", _repository.GetProjects());
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateProject()
        {
            if (ModelState.IsValid)
            {
                int projectId = _repository.CreateProject("New Project");

                if (projectId > 0)
                    return RedirectToAction("GetProjects");
            }

            return View("Error");
        }
        #endregion

        #region Project Details
        [Authorize]
        public ActionResult EditProjectDetails(int? projectId)
        {
            ViewBag.IsUserAdmin = (bool)System.Web.HttpContext.Current.Session["userIsAdmin"];
            ViewBag.UserId = (int)System.Web.HttpContext.Current.Session["userId"];

            if (projectId == null)
                return View("Error");

            Project project = _repository.GetProject(projectId.Value);

            if (project == null)
                return View("Error");

            return View("EditProject", project);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditProjectName(Project project)
        {
            if (project == null)
                return View("Error");

            if (ModelState.IsValid)
                _repository.UpdateProject(project);

            return RedirectToAction("EditProjectDetails", new { ProjectId = project.Id });
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteProject(int? projectid)
        {
            if (projectid == null)
                return View("Error");

            if (ModelState.IsValid)
                _repository.DeleteProject(projectid.Value);

            return RedirectToAction("GetProjects");
        }

        [Authorize]
        public ActionResult EditCategory(int? projectId, int? categoryId)
        {
            if (projectId == null)
                return View("Error");

            Category category = null;

            if (categoryId != null)
                category = _repository.GetCategory(categoryId.Value);

            EditCategoryViewModel viewModel = new EditCategoryViewModel { ProjectId = projectId };

            if (category != null)
            {
                viewModel.CategoryId = category.Id;
                viewModel.Name = category.Name;
                viewModel.Description = category.Description;
            }

            return View("EditCategory", viewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateCategory(EditCategoryViewModel viewModel)
        {
            if (viewModel == null || viewModel.ProjectId == null)
                return View("Error");

            if (ModelState.IsValid)
            {
                int categoryId = _repository.CreateCategory(viewModel.ProjectId.Value, new Category { Name = viewModel.Name, Description = viewModel.Description });

                if (categoryId > 0)
                    return RedirectToAction("EditProjectDetails", new { viewModel.ProjectId });
            }

            return View("Error");
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateCategory(EditCategoryViewModel viewModel)
        {
            if (viewModel == null || viewModel.ProjectId == null || viewModel.CategoryId == null)
                return View("Error");

            if (ModelState.IsValid)
                _repository.UpdateCategory(new Category { Id = viewModel.CategoryId.Value, Name = viewModel.Name, Description = viewModel.Description });

            return RedirectToAction("EditProjectDetails", new { ProjectId = viewModel.ProjectId.Value });
        }
        #endregion

        #region Sketch
        [Authorize]
        public ActionResult EditSketch(int? projectId, int? categoryId, int? sketchId)
        {
            if (projectId == null || categoryId == null)
                return View("Error");

            ViewBag.UserId = (int)System.Web.HttpContext.Current.Session["userId"];
            ViewBag.UserIsAdmin = (bool)System.Web.HttpContext.Current.Session["userIsAdmin"];

            Sketch sketch = null;

            if (sketchId != null)
                sketch = _repository.GetSketch(sketchId.Value);

            EditSketchViewModel viewModel = new EditSketchViewModel { ProjectId = projectId, CategoryId = categoryId };

            if (sketch != null)
            {
                viewModel.SketchId = sketch.Id;
                viewModel.UserId = sketch.User != null ? (int?)sketch.User.Id : null;
                viewModel.Data = sketch.Data;
                viewModel.ImageURI = sketch.ImageURI;
            }

            return View("EditSketch", viewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateSketch(EditSketchViewModel viewModel)
        {
            if (viewModel == null || viewModel.ProjectId == null || viewModel.CategoryId == null)
                return View("Error");

            if (ModelState.IsValid)
            {
                int userId = (int)System.Web.HttpContext.Current.Session["userId"];
                int sketchId = _repository.CreateSketch(viewModel.CategoryId.Value, userId, new Sketch { Data = viewModel.Data, ImageURI = viewModel.ImageURI });

                if (sketchId > 0)
                    return RedirectToAction("EditProjectDetails", new { ProjectId = viewModel.ProjectId.Value });
            }

            return View("Error");
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateSketch(EditSketchViewModel viewModel)
        {
            if (viewModel == null || viewModel.ProjectId == null || viewModel.SketchId == null)
                return View("Error");

            if (ModelState.IsValid)
                _repository.UpdateSketch(new Sketch { Id = viewModel.SketchId.Value, Data = viewModel.Data, ImageURI = viewModel.ImageURI });

            return RedirectToAction("EditProjectDetails", new { ProjectId = viewModel.ProjectId.Value });
        }
        #endregion
        #endregion
    }
}
