using System.Net;
using System.Web.Mvc;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces;
using System.Linq;
using CrowdDesign.Infrastructure.SQLServer;
using CrowdDesign.Infrastructure.SQLServer.Repositories;
using CrowdDesign.UI.Web.Models;
using CrowdDesign.UI.Web.Models.Project;
using System.Collections.Generic;

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
            if (System.Web.HttpContext.Current.Session["userIsAdmin"] != null)
                ViewBag.IsUserAdmin = (bool)System.Web.HttpContext.Current.Session["userIsAdmin"];
            else
                return RedirectToAction("Index", "Security");

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
            if (System.Web.HttpContext.Current.Session["userId"] != null)
            {
                ViewBag.UserId = (int)System.Web.HttpContext.Current.Session["userId"];
                ViewBag.IsUserAdmin = (bool)System.Web.HttpContext.Current.Session["userIsAdmin"];

                if (projectId == null)
                    return View("Error");

                Project project = _repository.GetProject(projectId.Value);

                if (project == null)
                    return View("Error");

                return View("EditProject", project);
            }

            return RedirectToAction("Index", "Security");
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
                dimension = _repository.GetDimensions(dimensionId.Value).SingleOrDefault();

            EditDimensionViewModel viewModel;

            if (dimension != null)
                viewModel = new EditDimensionViewModel(dimension);
            else
                viewModel = new EditDimensionViewModel { ProjectId = projectId };

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
                int dimensionId = _repository.CreateDimension(viewModel.ToDomainModel());

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
                _repository.UpdateDimension(viewModel.ToDomainModel());

            return RedirectToAction("EditProjectDetails", new { ProjectId = viewModel.ProjectId.Value });
        }
        #endregion

        #region Sketch
        [Authorize]
        public ActionResult EditSketch(int? projectId, int? dimensionId, int? sketchId)
        {
            if (projectId == null || dimensionId == null)
                return View("Error");

            if (System.Web.HttpContext.Current.Session["userId"] != null)
            {
                ViewBag.UserId = (int)System.Web.HttpContext.Current.Session["userId"];
                ViewBag.UserIsAdmin = (bool)System.Web.HttpContext.Current.Session["userIsAdmin"];

                Sketch sketch = null;

                if (sketchId != null)
                    sketch = _repository.GetSketch(sketchId.Value).SingleOrDefault();

                EditSketchViewModel viewModel;

                if (sketch != null)
                    viewModel = new EditSketchViewModel(sketch);
                else
                    viewModel = new EditSketchViewModel
                    {
                        ProjectId = projectId,
                        DimensionId = dimensionId
                    };

                return View("EditSketch", viewModel);
            }

            return RedirectToAction("Index", "Security");
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateSketch(EditSketchViewModel viewModel)
        {
            if (viewModel == null || viewModel.ProjectId == null || viewModel.DimensionId == null)
                return View("Error");

            if (ModelState.IsValid)
            {
                if (System.Web.HttpContext.Current.Session["userId"] != null)
                {
                    viewModel.UserId = (int)System.Web.HttpContext.Current.Session["userId"];

                    int sketchId = _repository.CreateSketch(viewModel.ToDomainModel());

                    if (sketchId > 0)
                        return RedirectToAction("EditProjectDetails", new { ProjectId = viewModel.ProjectId.Value });
                }
                else
                    return RedirectToAction("Index", "Security");
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
                _repository.UpdateSketch(viewModel.ToDomainModel());

            return RedirectToAction("EditProjectDetails", new { ProjectId = viewModel.ProjectId.Value });
        }

        [HttpPost]
        public JsonResult UpdateSketchDimension(int? dimensionId, int? sketchId)
        {
            if (dimensionId == null || sketchId == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return Json("Failed to move the sketch");
            }

            if (ModelState.IsValid)
            {
                Sketch sketch = _repository.GetSketch(sketchId.Value).SingleOrDefault();

                sketch.Dimension.Id = dimensionId.Value;

                _repository.UpdateSketch(sketch);
            }

            return Json("Sketch moved successfully");
        }

        [HttpPost]
        public JsonResult MergeDimensions(int? sourceDimensionId, int? targetDimensionId)
        {
            if (sourceDimensionId == null || targetDimensionId == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;

                return Json("Failed to merge dimensions");
            }

            if (ModelState.IsValid)
                _repository.MergeDimensions(sourceDimensionId.Value, targetDimensionId.Value);

            return Json("Dimensions merged successfully");
        }
        #endregion
        #endregion
    }
}
