namespace CrowdDesign.Core.Entities
{
    public class Sketch
    {
        #region Properties
        public int Id { get; set; }
        public User User { get; set; }
        public Category Category { get; set; }
        public string Data { get; set; } 
        #endregion
    }
}
