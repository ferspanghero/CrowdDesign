using CrowdDesign.Core.Entities;

namespace CrowdDesign.Core.Interfaces.Repositories
{
    /// <summary>
    /// Defines a repository of sketches of dimensions of a morphological chart.
    /// </summary>
    public interface ISketchRepository : IBaseRepository<Sketch, int>
    {
        #region Methods
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