using System.Collections.Generic;

namespace CrowdDesign.Core.Entities
{
    public class Project
    {
        #region Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Dimension> Dimensions { get; set; }
        #endregion
    }
}