using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrowdDesign.Core.Entities;

namespace CrowdDesign.Core.Interfaces.Repositories
{
    public interface IProjectVotingRepository : IBaseRepository<ProjectVotingEntry, int>
    {
        IDictionary<int, ProjectVotingEntry> GetUserProjectVotingEntries(int userId);
        IEnumerable<ProjectVotingEntry> Get(Func<ProjectVotingEntry, bool> predicate);
    }
}
