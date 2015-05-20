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
    public class DimensionController : BaseController<IDimensionRepository, Dimension, int>
    {
        #region Constructors
        public DimensionController()
            : base(new DimensionRepository(new DatabaseContext()))
        { }
        #endregion

        #region Methods
        public ActionResult EditDimension(int? projectId, int? dimensionId)
        {
            if (projectId == null)
                return View("Error");

            EditDimensionViewModel viewModel;

            if (dimensionId != null)
            {
                Dimension dimension = Repository.Get(dimensionId.Value).SingleOrDefault();

                if (dimension == null)
                    return View("Error");

                viewModel = new EditDimensionViewModel(dimension);
            }
            else
                viewModel = new EditDimensionViewModel { ProjectId = projectId };

            return View("EditDimension", viewModel);
        }

        [HttpPost]
        public ActionResult CreateDimension(EditDimensionViewModel viewModel)
        {
            if (viewModel != null && viewModel.ProjectId != null && ModelState.IsValid)
            {
                int dimensionId = Repository.Create(viewModel.ToDomainModel());

                if (dimensionId > 0)
                    return RedirectToAction("EditProject", "Project", new { viewModel.ProjectId });
            }

            return View("Error");
        }

        [HttpPost]
        public ActionResult UpdateDimension(EditDimensionViewModel viewModel)
        {
            if (viewModel != null && viewModel.ProjectId != null && viewModel.DimensionId != null && ModelState.IsValid)
            {
                Repository.Update(viewModel.ToDomainModel());

                return RedirectToAction("EditProject", "Project", new { ProjectId = viewModel.ProjectId.Value });
            }

            return View("Error");

        }

        [HttpPost]
        public JsonResult MergeDimensions(int? sourceDimensionId, int? targetDimensionId)
        {
            if (sourceDimensionId != null && targetDimensionId != null && ModelState.IsValid)
            {
                Repository.MergeDimensions(sourceDimensionId.Value, targetDimensionId.Value);

                return Json("Dimensions merged successfully");
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json("Failed to merge dimensions");
        }
        #endregion
    }
}
