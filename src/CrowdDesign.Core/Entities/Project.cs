using System.Collections.Generic;
using CrowdDesign.Core.Interfaces.Entities;

namespace CrowdDesign.Core.Entities
{
    /// <summary>
    /// Represents a design project that employs a morphological chart.
    /// </summary>
    public class Project : IDomainEntity<int>
    {
        #region Properties
        /// <summary>
        /// Gets or sets the project id.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the project name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the collection of dimensions of the morphological chart built in the project.
        /// </summary>
        public ICollection<Dimension> Dimensions { get; set; }
        #endregion
    }
}