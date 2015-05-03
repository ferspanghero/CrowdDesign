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
        #region Projects
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

        #region Dimensions
        /// <summary>
        /// Gets dimensions of a morphological chart by their id.
        /// </summary>
        /// <param name="dimensionIds">The ids of the desired dimensions.</param>
        /// <returns>A collection of dimensions that match the given ids.</returns>
        /// <remarks>If no ids are provided, all existing dimensions are returned</remarks>
        IEnumerable<Dimension> GetDimensions(params int[] dimensionIds);

        /// <summary>
        /// Creates a new dimension in a morphological chart.
        /// </summary>
        /// <param name="dimension">The new dimension to be created.</param>
        /// <returns>The new dimension id.</returns>
        int CreateDimension(Dimension dimension);

        /// <summary>
        /// Updates an existing dimension of a morphological chart
        /// </summary>
        /// <param name="dimension">The dimension to be updated.</param>
        void UpdateDimension(Dimension dimension);

        /// <summary>
        /// Deletes a dimension of a morphological chart.
        /// </summary>
        /// <param name="dimensionId">The id of the dimension to be deleted.</param>
        void DeleteDimension(int dimensionId);

        /// <summary>
        /// Merges dimensions of a morphological chart.
        /// </summary>
        /// <param name="dimensionIds">The ids of the dimensions to be merged.</param>
        void MergeDimensions(params int[] dimensionIds); 
        #endregion

        #region Sketches
        /// <summary>
        /// Gets solution sketches of dimensions of a morphological chart by their id.
        /// </summary>
        /// <param name="sketchIds">The ids of the desired sketches.</param>
        /// <returns>A collection of sketches that match the given ids.</returns>  
        /// <remarks>If no ids are provided, all existing sketches are returned</remarks>              
        IEnumerable<Sketch> GetSketches(params int[] sketchIds);

        /// <summary>
        /// Creates a new solution sketch in a dimension of a morphological chart.
        /// </summary>
        /// <param name="sketch">The sketch to be created.</param>
        /// <returns>The new sketch id.</returns>
        int CreateSketch(Sketch sketch);

        /// <summary>
        /// Updates an existing sketch of a dimension of a morphological chart.
        /// </summary>
        /// <param name="sketch">The sketch to be updated.</param>
        void UpdateSketch(Sketch sketch);

        /// <summary>
        /// Deletes a sketch of a dimension of morphological chart.
        /// </summary>
        /// <param name="sketchId">The id of the sketch to be deleted.</param>
        void DeleteSketch(int sketchId);

        /// <summary>
        /// Moves a sketch to the position of another sketch.
        /// </summary>
        /// <param name="sourceSketchId">The id of the sketch being moved.</param>
        /// <param name="targetSketchId">The id of the target sketch.</param>
        void ReplaceSketches(int sourceSketchId, int targetSketchId);

        /// <summary>
        /// Moves a sketch to the end position within a dimension.
        /// </summary>
        /// <param name="sourceSketchId">The id of the sketch being moved.</param>
        /// <param name="targetDimensionId">The id of the target dimension.</param>
        void MoveSketchToDimension(int sourceSketchId, int targetDimensionId);

        #endregion

        #endregion
    }
}