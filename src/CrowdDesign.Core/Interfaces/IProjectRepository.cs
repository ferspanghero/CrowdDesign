using System.Collections.Generic;
using CrowdDesign.Core.Entities;

namespace CrowdDesign.Core.Interfaces
{
    /// <summary>
    /// Defines a repository of projects and all data related to its morphological chart.
    /// </summary>
    public interface IProjectRepository
    {
        #region Methods
        /// <summary>
        /// Gets all the existing projects.
        /// </summary>
        /// <returns>A collection of all existing projets.</returns>
        IEnumerable<Project> GetProjects();
        
        /// <summary>
        /// Gets a project by its id.
        /// </summary>
        /// <param name="projectId">The id of the desired project.</param>
        /// <returns>A project that matches the given id.</returns>
        Project GetProject(int projectId);
        
        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="projectName">The name of the new project.</param>
        /// <returns>The new project id.</returns>
        int CreateProject(string projectName);
        
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
        
        /// <summary>
        /// Gets a dimension of a morphological chart by its id.
        /// </summary>
        /// <param name="dimensionId">The id of the desired dimension.</param>
        /// <returns>A dimension that matches the given id.</returns>
        Dimension GetDimension(int dimensionId);
        
        /// <summary>
        /// Creates a new dimension in a morphological chart.
        /// </summary>
        /// <param name="projectId">The id of the project which the morphological chart is part of.</param>
        /// <param name="dimension">The new dimension to be created.</param>
        /// <returns>The new dimension id.</returns>
        int CreateDimension(int projectId, Dimension dimension);
        
        /// <summary>
        /// Updates an existing dimension of a morphological chart
        /// </summary>
        /// <param name="dimension">The dimension to be updated.</param>
        void UpdateDimension(Dimension dimension);
        
        /// <summary>
        /// Gets a solution sketch of a dimension of a morphological chart by its id.
        /// </summary>
        /// <param name="sketchId">The id of the desired sketch.</param>
        /// <returns>A sketch that matches the given id.</returns>                
        Sketch GetSketch(int sketchId);
        
        /// <summary>
        /// Creates a new solution sketch in a dimension of a morphological chart.
        /// </summary>
        /// <param name="dimensionId">The dimension which the sketch is part of.</param>
        /// <param name="userId">The id of the user that authors the sketch.</param>
        /// <param name="sketch">The sketch to be created.</param>
        /// <returns>The new sketch id.</returns>
        int CreateSketch(int dimensionId, int userId, Sketch sketch);
        
        /// <summary>
        /// Updates an existing sketch of a dimension of a morphological chart.
        /// </summary>
        /// <param name="sketch">The sketch to be updated.</param>
        void UpdateSketch(Sketch sketch);
        #endregion
    }
}