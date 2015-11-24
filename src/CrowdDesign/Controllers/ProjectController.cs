using System;
using System.Collections;
using System.Linq;
using System.Web.Mvc;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces.Repositories;
using CrowdDesign.Infrastructure.SQLServer.Contexts;
using CrowdDesign.Infrastructure.SQLServer.Repositories;
using CrowdDesign.UI.Web.Models;
using CrowdDesign.Utils.AspNet.Mvc;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using CrowdDesign.Utils.Extensions;
using Microsoft.Ajax.Utilities;

namespace CrowdDesign.UI.Web.Controllers
{
    [Authorize]
    public class ProjectController : BaseController<IBaseRepository<Project, int>, Project, int>
    {
        #region Constructors
        public ProjectController()
            : base(new ProjectRepository(new DatabaseContext()))
        { }
        #endregion

        #region Methods
        public ActionResult GetProjects()
        {
            if (System.Web.HttpContext.Current.Session["userIsAdmin"] != null)
                ViewBag.IsUserAdmin = (bool)System.Web.HttpContext.Current.Session["userIsAdmin"];
            else
                return RedirectToAction("Index", "Security");

            return View("ManageProjects", Repository.Get());
        }

        [HttpPost]
        [DetectMultipleRequests]
        public ActionResult CreateProject()
        {
            if (ModelState.IsValid)
            {
                bool hasMultipleRequests = ViewData.ContainsKey("MultipleRequests");
                int projectId = -1;

                if (!hasMultipleRequests)
                    projectId = Repository.Create(new Project { Name = "New Project" });

                if (projectId > 0 || hasMultipleRequests)
                    return RedirectToAction("GetProjects");
            }

            return View("Error");
        }

        public ActionResult EditProject(int? projectId)
        {
            if (System.Web.HttpContext.Current.Session["userId"] != null)
            {
                if (projectId == null)
                    return View("Error");

                IDimensionVotingEntryRepository dimVotingRep = new DimensionVotingEntryRepository(new DatabaseContext());
                IProjectVotingRepository projVotingRep = new ProjectVotingEntryRepository(new DatabaseContext());
                IBaseRepository<User, int> userRep = new UserRepository(new DatabaseContext());

                Project project = Repository.Get(projectId.Value).SingleOrDefault();

                if (project == null)
                    return View("Error");

                int usersCount = userRep.Get(null).Count();
                IEnumerable<ProjectVotingEntry> projectVotingEntries = projVotingRep.Get(e => e.ProjectId == projectId.Value);
                int projectReadyVotesCount = projectVotingEntries.Count(e => e.Ready);

                // Initializes view model
                EditProjectViewModel viewModel = new EditProjectViewModel(project);

                viewModel.UserId = (int)System.Web.HttpContext.Current.Session["userId"];
                viewModel.IsUserAdmin = (bool)System.Web.HttpContext.Current.Session["userIsAdmin"];
                viewModel.UserDimensionVotingEntriesMap = dimVotingRep.GetUserDimensionVotingEntries(viewModel.UserId.Value, projectId.Value);
                viewModel.UserProjectVotingEntry = projVotingRep.Get(e => e.ProjectId == projectId.Value && e.UserId == viewModel.UserId.Value).SingleOrDefault();

                decimal userMinimumQuota = Math.Ceiling(usersCount / 2m);

                // Checking for at least 50% ready votes for project from the total number of users
                // If the project is ready for the voting phase
                if (projectReadyVotesCount >= userMinimumQuota)
                {
                    if (!project.Dimensions.IsNullOrEmpty())
                    {
                        // Gets the ids of all project dimensions
                        HashSet<int> dimensionIds = new HashSet<int>(project.Dimensions.Select(e => e.Id));

                        // Gets the ids of dimensions that are not eligible to go to the voting phase
                        HashSet<int> projectNonPassingDimensionsIds = new HashSet<int>
                        (
                            dimVotingRep
                                .Get(e => dimensionIds.Contains(e.DimensionId)) // Gets all voting entries for the project dimensions
                                .ToLookup(e => e.DimensionId, e => e.DownvotedDimension) // Creates a look up of the downvote entries
                                .Where(e => e.Count(d => d) >= userMinimumQuota) // Counts the number of downvotes and filters out dimensions that fulfill the downvote quota
                                .Select(e => e.Key) // Gets the filtered dimension ids
                        );

                        // Only keeps passing dimension ids
                        dimensionIds.RemoveWhere(e => projectNonPassingDimensionsIds.Contains(e));

                        var dimensions = viewModel.Dimensions.Where(e => dimensionIds.Contains(e.Id)).ToList();

                        // Gets the submitted sketches for the passing dimensions
                        foreach (var dimension in dimensions)
                        {
                            dimension.Sketches = dimension.Sketches.Where(s => s.Submitted).ToList();
                        }

                        // Assigns the passing dimensions to the view model
                        viewModel.Dimensions = new ReadOnlyCollection<Dimension>(dimensions);
                    }

                    viewModel.Phase = EditProjectViewModel.EProjectPhase.Voting;
                }
                // If the project is not ready for the voting phase (sketching phase)
                else
                {
                    viewModel.Phase = EditProjectViewModel.EProjectPhase.Sketching;
                }

                return View("EditProject", viewModel);
            }

            return RedirectToAction("Index", "Security");
        }

        [HttpPost]
        [DetectMultipleRequests]
        public ActionResult UpdateProject(Project project)
        {
            if (project == null || !ModelState.IsValid)
                return View("Error");

            if (!ViewData.ContainsKey("MultipleRequests"))
                Repository.Update(project);

            return RedirectToAction("EditProject", new { ProjectId = project.Id });
        }

        [HttpPost]
        [DetectMultipleRequests]
        public ActionResult DeleteProject(int? projectid)
        {
            if (projectid == null || !ModelState.IsValid)
                return View("Error");

            if (!ViewData.ContainsKey("MultipleRequests"))
                Repository.Delete(projectid.Value);

            return RedirectToAction("GetProjects");
        }

        [HttpPost]
        public JsonResult ChangeUserDimensionLikeStatus(int? userId, int? dimensionId, int? projectId, bool? downvote)
        {
            IDimensionVotingEntryRepository rep = new DimensionVotingEntryRepository(new DatabaseContext());
            DimensionVotingEntry entry = rep.Get(e => e.DimensionId == dimensionId.Value && e.UserId == userId.Value).SingleOrDefault();

            if (entry == null)
            {
                rep.Create(
                    new DimensionVotingEntry
                    {
                        DimensionId = dimensionId.Value,
                        ProjectId = projectId.Value,
                        UserId = userId.Value,
                        SelectedSketchId = null,
                        DownvotedDimension = downvote.Value
                    });
            }
            else
            {
                entry.DownvotedDimension = downvote.Value;

                rep.Update(entry);
            }

            return Json("Success");
        }

        [HttpPost]
        public JsonResult VoteForSketch(int? userId, int? dimensionId, int? projectId, int? sketchId)
        {
            IDimensionVotingEntryRepository rep = new DimensionVotingEntryRepository(new DatabaseContext());
            DimensionVotingEntry entry = rep.Get(e => e.DimensionId == dimensionId.Value && e.UserId == userId.Value).SingleOrDefault();

            if (entry == null || sketchId == null)
                return Json("Error");

            entry.SelectedSketchId = sketchId;

            rep.Update(entry);

            return Json("Success");
        }

        [HttpPost]
        public JsonResult ChangeUserProjectReadyStatus(int? userId, int? projectId, bool? ready)
        {
            IProjectVotingRepository rep = new ProjectVotingEntryRepository(new DatabaseContext());
            ProjectVotingEntry entry = rep.Get(e => e.ProjectId == projectId.Value && e.UserId == userId.Value).SingleOrDefault();

            if (entry == null)
            {
                rep.Create(
                    new ProjectVotingEntry
                    {
                        ProjectId = projectId.Value,
                        UserId = userId.Value,
                        Ready = ready.Value
                    });
            }
            else
            {
                entry.Ready = ready.Value;

                rep.Update(entry);
            }

            return Json("Success");
        }
        #endregion
    }
}
