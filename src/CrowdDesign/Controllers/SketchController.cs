using System.Linq;
using System.Net;
using System.Web.Mvc;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces.Repositories;
using CrowdDesign.Infrastructure.SQLServer.Contexts;
using CrowdDesign.Infrastructure.SQLServer.Repositories;
using CrowdDesign.UI.Web.Models;

namespace CrowdDesign.UI.Web.Controllers
{
    [Authorize]
    public class SketchController : Controller
    {
        #region Constructors
        public SketchController()
        {
            _repository = new SketchRepository(new DatabaseContext());
        }
        #endregion

        #region Fields
        private readonly ISketchRepository _repository;
        #endregion

        #region Methods
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
                        Sketch sketch = _repository.Get(sketchId.Value).SingleOrDefault();

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
        public ActionResult CreateSketch(EditSketchViewModel viewModel)
        {
            if (viewModel != null && viewModel.ProjectId != null && viewModel.DimensionId != null && ModelState.IsValid)
            {
                if (System.Web.HttpContext.Current.Session["userId"] != null)
                {
                    viewModel.UserId = (int)System.Web.HttpContext.Current.Session["userId"];

                    int sketchId = _repository.Create(viewModel.ToDomainModel());

                    if (sketchId > 0)
                        return RedirectToAction("EditProject", "Project", new { ProjectId = viewModel.ProjectId.Value });
                }
                else
                    return RedirectToAction("Index", "Security");
            }

            return View("Error");
        }

        [HttpPost]
        public ActionResult UpdateSketch(EditSketchViewModel viewModel)
        {
            if (viewModel != null && viewModel.ProjectId != null && viewModel.SketchId != null && ModelState.IsValid)
            {
                _repository.Update(viewModel.ToDomainModel());

                return RedirectToAction("EditProject", "Project", new { ProjectId = viewModel.ProjectId.Value });
            }

            return View("Error");
        }

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _repository.Dispose();

            base.Dispose(disposing);
        }
        #endregion
    }
}
