using CrowdDesign.Core.Interfaces.Entities;

namespace CrowdDesign.Core.Entities
{
    /// <summary>
    /// Represents a sketch of a solution candidate for a dimension in a morphological chart.
    /// </summary>
    public class Sketch : IDomainEntity<int>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the sketch id.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the dimension which the sketch is part of.
        /// </summary>
        public Dimension Dimension { get; set; }
        /// <summary>
        /// Gets or sets the user that authros the sketch.
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Gets or sets the sketch drawing data in a string format.
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// Gets or sets the URI of an image of the current state of the sketch.
        /// </summary>
        public string ImageUri { get; set; }
        /// <summary>
        /// Gets or sets the position of the sketch within the dimension it is part of.
        /// </summary>        
        public int Position { get; set; }
        /// <summary>
        /// Gets or sets the sketch title.
        /// </summary>
        public string Title { get; set; }
        #endregion
    }
}
