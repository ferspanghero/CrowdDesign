using System.Collections.Generic;

namespace CrowdDesign.Core.Entities
{
    public class User
    {
        #region Properties
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public string Name { get; set; }
        public ICollection<Sketch> Sketches { get; set; }
        #endregion
    }
}