using System.Linq;
using System.Net;
using System.Web.Mvc;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces.Repositories;
using CrowdDesign.Infrastructure.SQLServer.Contexts;
using CrowdDesign.Infrastructure.SQLServer.Repositories;
using CrowdDesign.UI.Web.Hubs;
using CrowdDesign.UI.Web.Models;
using CrowdDesign.Utils.AspNet.Mvc;
using Microsoft.AspNet.SignalR;

namespace CrowdDesign.UI.Web.Controllers
{
    [System.Web.Mvc.Authorize]
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
        [DetectMultipleRequests]
        public ActionResult CreateDimension(EditDimensionViewModel viewModel)
        {
            if (viewModel != null && viewModel.ProjectId != null && ModelState.IsValid)
            {
                bool hasMultipleRequests = ViewData.ContainsKey("MultipleRequests");
                int dimensionId = -1;

                // Prevents saving dimensions with the same name
                if (Repository.AnyEntity(d => d.Name.Equals(viewModel.Name)))
                {
                    ModelState.AddModelError("Name", "A dimension with the same name already exists");

                    return View("EditDimension", viewModel);
                }

                if (!hasMultipleRequests)
                {                    
                    dimensionId = Repository.Create(viewModel.ToDomainModel());

                    GlobalHost.ConnectionManager.GetHubContext<MorphologicalChartHub>().Clients.All.refresh();
                }

                if (dimensionId > 0 || hasMultipleRequests)
                    return RedirectToAction("EditProject", "Project", new { viewModel.ProjectId });
            }

            return View("Error");
        }

        [HttpPost]
        [DetectMultipleRequests]
        public ActionResult UpdateDimension(EditDimensionViewModel viewModel)
        {
            if (viewModel != null && viewModel.ProjectId != null && viewModel.DimensionId != null && ModelState.IsValid)
            {
                // Prevents saving dimensions with the same name
                if (Repository.AnyEntity(d => d.Name.Equals(viewModel.Name) && d.Id != viewModel.DimensionId))
                {
                    ModelState.AddModelError("Name", "A dimension with the same name already exists");

                    return View("EditDimension", viewModel);
                }

                if (!ViewData.ContainsKey("MultipleRequests"))
                {                    
                    Repository.Update(viewModel.ToDomainModel());

                    GlobalHost.ConnectionManager.GetHubContext<MorphologicalChartHub>().Clients.All.refresh();
                }

                return RedirectToAction("EditProject", "Project", new { ProjectId = viewModel.ProjectId.Value });
            }

            return View("Error");

        }

        [HttpPost]
        [DetectMultipleRequests]
        public ActionResult DeleteDimension(int? dimensionId, int? projectId)
        {
            if (dimensionId == null || projectId == null || !ModelState.IsValid)
                return View("Error");

            if (!ViewData.ContainsKey("MultipleRequests"))
            {
                Repository.Delete(dimensionId.Value);

                GlobalHost.ConnectionManager.GetHubContext<MorphologicalChartHub>().Clients.All.refresh();
            }

            return RedirectToAction("EditProject", "Project", new { ProjectId = projectId.Value });
        }

        [HttpPost]
        [DetectMultipleRequests]
        public JsonResult MergeDimensions(int? sourceDimensionId, int? targetDimensionId)
        {
            if (sourceDimensionId != null && targetDimensionId != null && ModelState.IsValid)
            {
                if (!ViewData.ContainsKey("MultipleRequests"))
                    Repository.MergeDimensions(sourceDimensionId.Value, targetDimensionId.Value);

                GlobalHost.ConnectionManager.GetHubContext<MorphologicalChartHub>().Clients.All.refresh();

                return Json("Dimensions merged successfully");
            }

            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return Json("Failed to merge dimensions");
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
