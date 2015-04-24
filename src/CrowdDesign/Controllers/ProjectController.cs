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
        public ActionResult EditDimension(int? projectId, int? dimensionId)
        {
            if (projectId == null)
                return View("Error");

            Dimension dimension = null;

            if (dimensionId != null)
                dimension = _repository.GetDimension(dimensionId.Value);

            EditDimensionViewModel viewModel = new EditDimensionViewModel { ProjectId = projectId };

            if (dimension != null)
            {
                viewModel.DimensionId = dimension.Id;
                viewModel.Name = dimension.Name;
                viewModel.Description = dimension.Description;
            }

            return View("EditDimension", viewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateDimension(EditDimensionViewModel viewModel)
        {
            if (viewModel == null || viewModel.ProjectId == null)
                return View("Error");

            if (ModelState.IsValid)
            {
                int dimensionId = _repository.CreateDimension(viewModel.ProjectId.Value, new Dimension { Name = viewModel.Name, Description = viewModel.Description });

                if (dimensionId > 0)
                    return RedirectToAction("EditProjectDetails", new { viewModel.ProjectId });
            }

            return View("Error");
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateDimension(EditDimensionViewModel viewModel)
        {
            if (viewModel == null || viewModel.ProjectId == null || viewModel.DimensionId == null)
                return View("Error");

            if (ModelState.IsValid)
                _repository.UpdateDimension(new Dimension { Id = viewModel.DimensionId.Value, Name = viewModel.Name, Description = viewModel.Description });

            return RedirectToAction("EditProjectDetails", new { ProjectId = viewModel.ProjectId.Value });
        }
        #endregion

        #region Sketch
        [Authorize]
        public ActionResult EditSketch(int? projectId, int? dimensionId, int? sketchId)
        {
            if (projectId == null || dimensionId == null)
                return View("Error");

            ViewBag.UserId = (int)System.Web.HttpContext.Current.Session["userId"];
            ViewBag.UserIsAdmin = (bool)System.Web.HttpContext.Current.Session["userIsAdmin"];

            Sketch sketch = null;

            if (sketchId != null)
                sketch = _repository.GetSketch(sketchId.Value);

            EditSketchViewModel viewModel = new EditSketchViewModel { ProjectId = projectId, DimensionId = dimensionId };

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
            if (viewModel == null || viewModel.ProjectId == null || viewModel.DimensionId == null)
                return View("Error");

            if (ModelState.IsValid)
            {
                int userId = (int)System.Web.HttpContext.Current.Session["userId"];
                int sketchId = _repository.CreateSketch(viewModel.DimensionId.Value, userId, new Sketch { Data = viewModel.Data, ImageURI = viewModel.ImageURI });

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
