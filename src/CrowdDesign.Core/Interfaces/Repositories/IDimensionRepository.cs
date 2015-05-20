using CrowdDesign.Core.Entities;

namespace CrowdDesign.Core.Interfaces.Repositories
{
    /// <summary>
    /// Defines a repository of dimensions of a morphological chart.
    /// </summary>
    public interface IDimensionRepository : IBaseRepository<Dimension, int>
    {
        #region Methods
        /// <summary>
        /// Merges dimensions of a morphological chart.
        /// </summary>
        /// <param name="dimensionIds">The ids of the dimensions to be merged.</param>
        void MergeDimensions(params int[] dimensionIds);
        #endregion
    }
}