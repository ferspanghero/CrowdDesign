using System.Linq;
using System.Net;
using System.Web.Mvc;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Exceptions;
using CrowdDesign.Core.Interfaces.Repositories;
using CrowdDesign.Infrastructure.SQLServer.Contexts;
using CrowdDesign.Infrastructure.SQLServer.Repositories;
using CrowdDesign.UI.Web.Models;
using CrowdDesign.Utils.AspNet.Mvc;
using Microsoft.AspNet.SignalR;
using CrowdDesign.UI.Web.Hubs;

namespace CrowdDesign.UI.Web.Controllers
{
    [System.Web.Mvc.Authorize]
    public class SketchController : BaseController<ISketchRepository, Sketch, int>
    {
        #region Constructors
        public SketchController()
            : base(new SketchRepository(new DatabaseContext()))
        {
        }
        #endregion

        #region Methods
        public ActionResult EditSketch(int? projectId, int? dimensionId, int? sketchId)
        {
            if (projectId != null && dimensionId != null)
            {
                // Since an action is being executed after the session expires (and going to the Index page after that),
                // this check is necessary to prevent a NullReferenceException
                if (System.Web.HttpContext.Current.Session["userId"] != null)
                {
                    ViewBag.UserId = (int)System.Web.HttpContext.Current.Session["userId"];
                    ViewBag.UserIsAdmin = (bool)System.Web.HttpContext.Current.Session["userIsAdmin"];

                    EditSketchViewModel viewModel;

                    if (sketchId != null)
                    {
                        Sketch sketch = Repository.Get(sketchId.Value).SingleOrDefault();

                        if (sketch == null)
                            return View("Error");

                        viewModel = new EditSketchViewModel(sketch);
                    }
                    else
                    {
                        // TODO: Avoid loading the entire dimension with its sketches only to get the sketches count
                        IDimensionRepository dimensionRepository = new DimensionRepository(new DatabaseContext());
                        Dimension dimension = dimensionRepository.Get(dimensionId.Value).SingleOrDefault();

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

        [HttpPost]
        [DetectMultipleRequests]
        public ActionResult CreateSketch(EditSketchViewModel viewModel)
        {
            if (viewModel != null && viewModel.ProjectId != null && viewModel.DimensionId != null && ModelState.IsValid)
            {
                // Since an action is being executed after the session expires (and going to the Index page after that),
                // this check is necessary to prevent a NullReferenceException
                if (System.Web.HttpContext.Current.Session["userId"] != null)
                {
                    viewModel.UserId = (int)System.Web.HttpContext.Current.Session["userId"];

                    bool hasMultipleRequests = ViewData.ContainsKey("MultipleRequests");
                    int sketchId = -1;

                    if (!hasMultipleRequests)
                    {
                        sketchId = Repository.Create(viewModel.ToDomainModel());

                        GlobalHost.ConnectionManager.GetHubContext<MorphologicalChartHub>().Clients.All.refresh();
                    }

                    if (sketchId > 0 || hasMultipleRequests)
                        return RedirectToAction("EditProject", "Project", new { ProjectId = viewModel.ProjectId.Value });
                }
                else
                    return RedirectToAction("Index", "Security");
            }

            return View("Error");
        }

        [HttpPost]
        [DetectMultipleRequests]
        public ActionResult UpdateSketch(EditSketchViewModel viewModel)
        {
            if (viewModel != null && viewModel.ProjectId != null && viewModel.SketchId != null && ModelState.IsValid)
            {
                if (!ViewData.ContainsKey("MultipleRequests"))
                {
                    try
                    {
                        Repository.Update(viewModel.ToDomainModel());
                    }
                    catch (EntityAlreadyDeletedException ex)
                    {
                        ModelState.AddModelError("Title", ex.Message);

                        return RedirectToAction("EditSketch", new { viewModel.ProjectId, viewModel.DimensionId, viewModel.SketchId });
                    }

                    GlobalHost.ConnectionManager.GetHubContext<MorphologicalChartHub>().Clients.All.refresh();
                }

                return RedirectToAction("EditProject", "Project", new { ProjectId = viewModel.ProjectId.Value });
            }

            return View("Error");
        }

        [HttpPost]
        [DetectMultipleRequests]
        public ActionResult DeleteSketch(int? sketchId, int? projectId)
        {
            if (sketchId == null || projectId == null || !ModelState.IsValid)
                return View("Error");

            if (!ViewData.ContainsKey("MultipleRequests"))
            {
                Repository.Delete(sketchId.Value);

                GlobalHost.ConnectionManager.GetHubContext<MorphologicalChartHub>().Clients.All.refresh();
            }


            return RedirectToAction("EditProject", "Project", new { ProjectId = projectId.Value });
        }

        [HttpPost]
        [DetectMultipleRequests]
        public JsonResult ReplaceSketches(int? sourceSketchId, int? targetSketchId)
        {
            if (sourceSketchId != null && targetSketchId != null && ModelState.IsValid)
            {
                if (!ViewData.ContainsKey("MultipleRequests"))
                    Repository.ReplaceSketches(sourceSketchId.Value, targetSketchId.Value);

                GlobalHost.ConnectionManager.GetHubContext<MorphologicalChartHub>().Clients.All.refresh();

                return Json("Sketch moved successfully");
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json("Failed to move the sketch");
        }

        [HttpPost]
        [DetectMultipleRequests]
        public ActionResult MoveSketchToDimension(int? sourceSketchId, int? targetDimensionId)
        {
            if (sourceSketchId != null && targetDimensionId != null && ModelState.IsValid)
            {
                if (!ViewData.ContainsKey("MultipleRequests"))
                    Repository.MoveSketchToDimension(sourceSketchId.Value, targetDimensionId.Value);

                GlobalHost.ConnectionManager.GetHubContext<MorphologicalChartHub>().Clients.All.refresh();

                return Json("Sketch moved successfully");
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json("Failed to move the sketch");
        }
        #endregion

        public ActionResult CancelAction(int? projectId)
        {
            if (projectId == null || !ModelState.IsValid)
                return View("Error");

            return RedirectToAction("EditProject", "Project", new { ProjectId = projectId.Value });
        }
    }
}
