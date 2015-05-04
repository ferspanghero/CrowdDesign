using System.Collections.Generic;

namespace CrowdDesign.Core.Entities
{
    /// <summary>
    /// Represents a dimension of morphological chart..
    /// </summary>
    public class Dimension
    {
        #region Properties
        /// <summary>
        /// Gets or sets the dimension id.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the project which the dimension is part of.
        /// </summary>
        public Project Project { get; set; }
        /// <summary>
        /// Gets or sets the dimension name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the dimension description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the name of the criteria by which the solutions sketches of the dimension are sorted.
        /// </summary>
        public string SortCriteria { get; set; }
        /// <summary>
        /// Gets or sets the collection of solutions sketches of the dimension.
        /// </summary>
        public ICollection<Sketch> Sketches { get; set; }
        #endregion        
    }
}
