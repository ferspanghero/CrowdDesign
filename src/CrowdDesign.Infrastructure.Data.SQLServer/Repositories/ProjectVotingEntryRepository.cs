using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces.Repositories;

namespace CrowdDesign.Infrastructure.SQLServer.Repositories
{
    public class ProjectVotingEntryRepository : BaseRepository<ProjectVotingEntry, int>
    {
        public ProjectVotingEntryRepository(DbContext context)
            : base(context)
        {
        }
    }
}
