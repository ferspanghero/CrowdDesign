using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrowdDesign.Core.Entities;

namespace CrowdDesign.Core.Interfaces.Repositories
{
    public interface IDimensionVotingEntryRepository : IBaseRepository<DimensionVotingEntry, int>
    {
        IDictionary<int, DimensionVotingEntry> GetUserDimensionVotingEntries(int userId, int projectId);
        IEnumerable<DimensionVotingEntry> Get(Func<DimensionVotingEntry, bool> predicate);
    }
}
