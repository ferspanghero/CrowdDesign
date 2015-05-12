using System.Collections.Generic;
using CrowdDesign.Core.Entities;
using System;

namespace CrowdDesign.Core.Interfaces
{
    /// <summary>
    /// Defines a repository of sketches of dimensions of a morphological chart.
    /// </summary>
    public interface ISketchRepository : IDisposable
    {
        #region Methods
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
    }
}