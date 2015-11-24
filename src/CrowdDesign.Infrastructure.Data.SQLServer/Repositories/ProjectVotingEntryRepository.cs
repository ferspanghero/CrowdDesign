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
    public class ProjectVotingEntryRepository : BaseRepository<ProjectVotingEntry, int>, IProjectVotingRepository
    {
        public ProjectVotingEntryRepository(DbContext context)
            : base(context)
        {
        }

        public IDictionary<int, ProjectVotingEntry> GetUserProjectVotingEntries(int userId)
        {
            var query = from e in EntitySet
                        where e.UserId == userId
                        select e;

            return query.ToDictionary(k => k.ProjectId, v => v);
        }

        public IEnumerable<ProjectVotingEntry> Get(Func<ProjectVotingEntry, bool> predicate)
        {
            return EntitySet.Where(predicate).Select(e => e);
        }
    }
}
