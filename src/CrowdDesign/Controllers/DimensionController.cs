﻿using System.Linq;
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
using System;
using CrowdDesign.Core.Exceptions;

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
        [ImportModelStateFromTempData]
        public ActionResult EditDimension(int? projectId, int? dimensionId)
        {
            if (projectId == null)
                return View("Error");

            EditDimensionViewModel viewModel;

            if (dimensionId != null)
            {
                Dimension dimension = Repository.Get(dimensionId.Value).SingleOrDefault();

                if (dimension == null)
                {
                    // If a previous action caused the model to be invalid (like a failed update or creation), return the view so that it can display the errors
                    if (!ModelState.IsValid)
                        viewModel = new EditDimensionViewModel { ProjectId = projectId };
                    else
                        return View("Error");
                }
                else
                    viewModel = new EditDimensionViewModel(dimension);
            }
            else
                viewModel = new EditDimensionViewModel { ProjectId = projectId };

            return View("EditDimension", viewModel);
        }

        [HttpPost]
        [DetectMultipleRequests]
        [ExportModelStateToTempData]
        public ActionResult CreateDimension(EditDimensionViewModel viewModel)
        {
            if (viewModel != null && viewModel.ProjectId != null && ModelState.IsValid)
            {
                bool hasMultipleRequests = ViewData.ContainsKey("MultipleRequests");
                int dimensionId = -1;

                if (!hasMultipleRequests)
                {
                    try
                    {
                        dimensionId = Repository.Create(viewModel.ToDomainModel());
                    }
                    catch (EntityAlreadyExistsException ex)
                    {
                        ModelState.AddModelError("Name", ex.Message);

                        return RedirectToAction("EditDimension", new { viewModel.ProjectId, viewModel.DimensionId });
                    }

                    GlobalHost.ConnectionManager.GetHubContext<MorphologicalChartHub>().Clients.All.refresh();
                }

                if (dimensionId > 0 || hasMultipleRequests)
                    return RedirectToAction("EditProject", "Project", new { viewModel.ProjectId });
            }

            return View("Error");
        }

        [HttpPost]
        [DetectMultipleRequests]
        [ExportModelStateToTempData]
        public ActionResult UpdateDimension(EditDimensionViewModel viewModel)
        {
            if (viewModel != null && viewModel.ProjectId != null && viewModel.DimensionId != null && ModelState.IsValid)
            {
                if (!ViewData.ContainsKey("MultipleRequests"))
                {
                    try
                    {
                        Repository.Update(viewModel.ToDomainModel());
                    }
                    catch (EntityAlreadyExistsException ex)
                    {
                        ModelState.AddModelError("Name", ex.Message);

                        return RedirectToAction("EditDimension", new { viewModel.ProjectId, viewModel.DimensionId });
                    }
                    catch (EntityAlreadyDeletedException ex)
                    {
                        ModelState.AddModelError("Name", ex.Message);

                        return RedirectToAction("EditDimension", new { viewModel.ProjectId, viewModel.DimensionId });
                    }

                    GlobalHost.ConnectionManager.GetHubContext<MorphologicalChartHub>().Clients.All.refresh();
                }

                return RedirectToAction("EditProject", "Project", new { viewModel.ProjectId });
            }

            return View("Error");

        }

        [HttpPost]
        [DetectMultipleRequests]
        [ExportModelStateToTempData]
        public ActionResult DeleteDimension(int? dimensionId, int? projectId)
        {
            if (dimensionId == null || projectId == null || !ModelState.IsValid)
                return View("Error");

            if (!ViewData.ContainsKey("MultipleRequests"))
            {
                try
                {
                    Repository.Delete(dimensionId.Value);
                }
                catch (EntityNotFoundException ex)
                {
                    ModelState.AddModelError("Name", ex.Message);

                    return RedirectToAction("EditDimension", new { projectId, dimensionId });
                }

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

        public ActionResult CancelAction(int? projectId)
        {
            if (projectId == null || !ModelState.IsValid)
                return View("Error");
            return RedirectToAction("EditProject", "Project", new { ProjectId = projectId.Value });
        }
        #endregion       
    }
}
