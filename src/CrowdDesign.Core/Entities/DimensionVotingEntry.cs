using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrowdDesign.Core.Interfaces.Entities;

namespace CrowdDesign.Core.Entities
{
    public class DimensionVotingEntry : IDomainEntity<int>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int DimensionId { get; set; }
        public int ProjectId { get; set; }
        public bool DownvotedDimension { get; set; } 
        public int? SelectedSketchId { get; set; }        
    }
}
