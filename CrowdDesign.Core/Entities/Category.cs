namespace CrowdDesign.Core.Entities
{
    public class Category
    {
        #region Properties
        public int Id { get; set; }
        public Project Project { get; set; }
        public string Name { get; set; } 
        #endregion
    }
}
