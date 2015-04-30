using CrowdDesign.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace CrowdDesign.UI.Web.Models.Project
{
    public class EditDimensionViewModel
    {
        #region Constructors
        public EditDimensionViewModel()
            : this(null)
        { }

        public EditDimensionViewModel(Dimension dimension)
        {
            if (dimension != null)
            {
                DimensionId = dimension.Id;
                Name = dimension.Name;
                Description = dimension.Description;
                SortCriteria = dimension.SortCriteria;

                if (dimension.Project != null)
                    ProjectId = dimension.Project.Id;
            }
        }
        #endregion

        #region Properties
        public int? ProjectId { get; set; }
        public int? DimensionId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(200)]
        public string Description { get; set; }
        [Display(Name="Sort criteria")]
        [StringLength(50)]
        public string SortCriteria { get; set; }
        #endregion

        #region Methods
        public Dimension ToDomainModel()
        {
            Dimension dimension = new Dimension();

            dimension.Id = DimensionId ?? -1;
            dimension.Name = Name;
            dimension.Description = Description;
            dimension.SortCriteria = SortCriteria;

            dimension.Project = new Core.Entities.Project { Id = ProjectId ?? -1 };

            return
                dimension;
        }
        #endregion
    }
}