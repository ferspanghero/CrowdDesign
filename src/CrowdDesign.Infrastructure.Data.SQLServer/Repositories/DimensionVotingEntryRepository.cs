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
    public class DimensionVotingEntryRepository : BaseRepository<DimensionVotingEntry, int>, IDimensionVotingEntryRepository
    {
        public DimensionVotingEntryRepository(DbContext context)
            : base(context)
        {
        }

        public IDictionary<int, DimensionVotingEntry> GetUserDimensionVotingEntries(int userId, int projectId)
        {
            var query = from e in EntitySet
                        where e.UserId == userId && e.ProjectId == projectId
                        select e;

            return query.ToDictionary(k => k.DimensionId, v => v);
        }

        public IEnumerable<DimensionVotingEntry> Get(Func<DimensionVotingEntry, bool> predicate)
        {
            return EntitySet.Where(predicate).Select(e => e);
        }
    }
}
