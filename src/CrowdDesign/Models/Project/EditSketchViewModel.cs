namespace CrowdDesign.UI.Web.Models.Project
{
    public class EditSketchViewModel
    {
        #region Properties
        public int? ProjectId { get; set; }
        public int? DimensionId { get; set; }
        public int? SketchId { get; set; }
        public int? UserId { get; set; }
        public string Data { get; set; }
        public string ImageUri { get; set; }
        #endregion        
    }
}