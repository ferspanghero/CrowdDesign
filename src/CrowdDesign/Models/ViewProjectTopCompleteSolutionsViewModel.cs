using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CrowdDesign.Core.Entities;

namespace CrowdDesign.UI.Web.Models
{
    public class ViewProjectTopCompleteSolutionsViewModel
    {
        #region Constructors
        public ViewProjectTopCompleteSolutionsViewModel(Project project, IEnumerable<KeyValuePair<IEnumerable<int?>, int>> topCompleteSolutions)
        {
            this.TopCompleteSolutions = topCompleteSolutions;

            if (project != null)
            {
                ProjectId = project.Id;
                Name = project.Name;

                if (project.Dimensions != null)
                {
                    // Forces dimensions to be materialized
                    Dimensions = new ReadOnlyCollection<Dimension>(project.Dimensions.ToList());
                }
            }
        }
        #endregion

        #region Properties        
        public int? ProjectId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public IReadOnlyCollection<Dimension> Dimensions { get; set; }
        public IEnumerable<KeyValuePair<IEnumerable<int?>, int>> TopCompleteSolutions { get; set; }
        #endregion
    }
}