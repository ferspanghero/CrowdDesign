namespace CrowdDesign.Core.Entities
{
    public class Sketch
    {
        #region Properties
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public byte[] Image { get; set; } 
        #endregion
    }
}
