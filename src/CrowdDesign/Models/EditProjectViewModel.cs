using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CrowdDesign.Core.Entities;

namespace CrowdDesign.UI.Web.Models
{
    public class EditProjectViewModel
    {
        #region Enums
        public enum EProjectPhase
        {
            Unknown = 0,
            Sketching = 1,
            Voting = 2
        }
        #endregion

        #region Constructors
        public EditProjectViewModel()
            : this(null)
        { }

        public EditProjectViewModel(Project project)
        {
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
        public int? UserId { get; set; }
        public bool IsUserAdmin { get; set; }
        public EProjectPhase Phase { get; set; }
        public IDictionary<int, DimensionVotingEntry> UserDimensionVotingEntriesMap { get; set; }
        public ProjectVotingEntry UserProjectVotingEntry { get; set; }
        #endregion
    }
}