using System.Collections.Generic;

namespace CrowdDesign.Core.Entities
{
    public class Dimension
    {
        #region Properties
        public int Id { get; set; }
        public Project Project { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Sketch> Sketches { get; set; }
        #endregion        
    }
}
