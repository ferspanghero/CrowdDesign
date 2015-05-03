using System.Linq;
using System.Net;
using System.Web.Mvc;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces;
using CrowdDesign.Infrastructure.SQLServer.Repositories;
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
                int projectId = _repository.CreateProject(new Project { Name = "New Project" });

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

                Project project = _repository.GetProjects(projectId.Value).SingleOrDefault();

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
            if (project == null || !ModelState.IsValid)
                return View("Error");

            _repository.UpdateProject(project);

            return RedirectToAction("EditProjectDetails", new { ProjectId = project.Id });
        }

        [Authorize]
        [HttpPost]
        public ActionResult DeleteProject(int? projectid)
        {
            if (projectid == null || !ModelState.IsValid)
                return View("Error");

            _repository.DeleteProject(projectid.Value);

            return RedirectToAction("GetProjects");
        }

        [Authorize]
        public ActionResult EditDimension(int? projectId, int? dimensionId)
        {
            if (projectId == null)
                return View("Error");

            EditDimensionViewModel viewModel;

            if (dimensionId != null)
            {
                Dimension dimension = _repository.GetDimensions(dimensionId.Value).SingleOrDefault();

                if (dimension == null)
                    return View("Error");

                viewModel = new EditDimensionViewModel(dimension);
            }
            else
                viewModel = new EditDimensionViewModel { ProjectId = projectId };

            return View("EditDimension", viewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateDimension(EditDimensionViewModel viewModel)
        {
            if (viewModel != null && viewModel.ProjectId != null && ModelState.IsValid)
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
            if (viewModel != null && viewModel.ProjectId != null && viewModel.DimensionId != null && ModelState.IsValid)
            {
                _repository.UpdateDimension(viewModel.ToDomainModel());

                return RedirectToAction("EditProjectDetails", new { ProjectId = viewModel.ProjectId.Value });
            }

            return View("Error");

        }
        #endregion

        #region Sketch
        [Authorize]
        public ActionResult EditSketch(int? projectId, int? dimensionId, int? sketchId)
        {
            if (projectId != null && dimensionId != null)
            {
                if (System.Web.HttpContext.Current.Session["userId"] != null)
                {
                    ViewBag.UserId = (int)System.Web.HttpContext.Current.Session["userId"];
                    ViewBag.UserIsAdmin = (bool)System.Web.HttpContext.Current.Session["userIsAdmin"];

                    EditSketchViewModel viewModel;

                    if (sketchId != null)
                    {
                        Sketch sketch = _repository.GetSketches(sketchId.Value).SingleOrDefault();

                        if (sketch == null)
                            return View("Error");

                        viewModel = new EditSketchViewModel(sketch);
                    }
                    else
                    {
                        // TODO: Avoid loading the entire dimension with its sketches only to get the sketches count
                        Dimension dimension = _repository.GetDimensions(dimensionId.Value).SingleOrDefault();

                        if (dimension == null || dimension.Sketches == null)
                            return View("Error");

                        viewModel = new EditSketchViewModel
                        {
                            ProjectId = projectId,
                            DimensionId = dimensionId,
                            Position = dimension.Sketches.Count + 1
                        };
                    }

                    return View("EditSketch", viewModel);
                }

                return RedirectToAction("Index", "Security");
            }

            return View("Error");
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateSketch(EditSketchViewModel viewModel)
        {
            if (viewModel != null && viewModel.ProjectId != null && viewModel.DimensionId != null && ModelState.IsValid)
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
            if (viewModel != null && viewModel.ProjectId != null && viewModel.SketchId != null && ModelState.IsValid)
            {
                _repository.UpdateSketch(viewModel.ToDomainModel());

                return RedirectToAction("EditProjectDetails", new { ProjectId = viewModel.ProjectId.Value });
            }

            return View("Error");
        }

        [Authorize]
        [HttpPost]
        public JsonResult ReplaceSketches(int? sourceSketchId, int? targetSketchId)
        {
            if (sourceSketchId != null && targetSketchId != null && ModelState.IsValid)
            {
                _repository.ReplaceSketches(sourceSketchId.Value, targetSketchId.Value);

                return Json("Sketch moved successfully");
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json("Failed to move the sketch");
        }

        [Authorize]
        [HttpPost]
        public ActionResult MoveSketchToDimension(int? sourceSketchId, int? targetDimensionId)
        {
            if (sourceSketchId != null && targetDimensionId != null && ModelState.IsValid)
            {
                _repository.MoveSketchToDimension(sourceSketchId.Value, targetDimensionId.Value);

                return Json("Sketch moved successfully");
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json("Failed to move the sketch");
        }

        [Authorize]
        [HttpPost]
        public JsonResult MergeDimensions(int? sourceDimensionId, int? targetDimensionId)
        {
            if (sourceDimensionId != null && targetDimensionId != null && ModelState.IsValid)
            {
                _repository.MergeDimensions(sourceDimensionId.Value, targetDimensionId.Value);

                return Json("Dimensions merged successfully");
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json("Failed to merge dimensions");
        }
        #endregion
        #endregion
    }
}
