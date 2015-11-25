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
    public class DimensionVotingEntryRepository : BaseRepository<DimensionVotingEntry, int>
    {
        public DimensionVotingEntryRepository(DbContext context)
            : base(context)
        {
        }
    }
}
