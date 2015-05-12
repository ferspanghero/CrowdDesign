using System.Collections.Generic;
using CrowdDesign.Core.Entities;
using System;

namespace CrowdDesign.Core.Interfaces
{
    /// <summary>
    /// Defines a repository of projects that contain a morphological chart.
    /// </summary>
    public interface IProjectRepository : IDisposable
    {
        #region Methods
        /// <summary>
        /// Gets projects by their id.
        /// </summary>
        /// <param name="projectIds">The ids of the desired projects.</param>
        /// <returns>A collection of projects that match the given ids.</returns>
        /// <remarks>If no ids are provided, all existing projects are returned</remarks>
        IEnumerable<Project> GetProjects(params int[] projectIds);

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="project">The new project to be created.</param>
        /// <returns>The new project id.</returns>
        int CreateProject(Project project);

        /// <summary>
        /// Updates an existing project.
        /// </summary>
        /// <param name="project">The project to be updated.</param>
        void UpdateProject(Project project);

        /// <summary>
        /// Deletes a project.
        /// </summary>
        /// <param name="projectId">The id of the project to be deleted.</param>
        void DeleteProject(int projectId);
        #endregion
    }
}