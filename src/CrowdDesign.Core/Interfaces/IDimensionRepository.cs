using System.Collections.Generic;
using CrowdDesign.Core.Entities;
using System;

namespace CrowdDesign.Core.Interfaces
{
    /// <summary>
    /// Defines a repository of dimensions of a morphological chart.
    /// </summary>
    public interface IDimensionRepository : IDisposable
    {
        #region Methods
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
    }
}