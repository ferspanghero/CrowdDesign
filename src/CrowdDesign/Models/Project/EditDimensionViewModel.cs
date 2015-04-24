using System.ComponentModel.DataAnnotations;

namespace CrowdDesign.UI.Web.Models.Project
{
    public class EditDimensionViewModel
    {
        #region Properties
        public int? ProjectId { get; set; }
        public int? DimensionId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(200)]
        public string Description { get; set; }
        #endregion        
    }
}