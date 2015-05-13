using System.Data.Entity;
using System.Linq;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces.Repositories;
using CrowdDesign.Infrastructure.SQLServer.Resources;

namespace CrowdDesign.Infrastructure.SQLServer.Repositories
{
    public class SecurityRepository : BaseRepository<User, int>, ISecurityRepository
    {
        #region Constructors
        public SecurityRepository(DbContext context)
            : base(context)
        { }
        #endregion

        #region Properties
        protected override string EntityNotFoundMessage
        {
            get { return SecurityStrings.UserNotFound; }
        }
        #endregion

        #region Methods
        protected override IQueryable<User> GetRelatedEntities(IQueryable<User> entitiesQuery)
        {
            return
                entitiesQuery
                    .Include(e => e.Sketches);
        }

        public User Login(string userName, string password)
        {
            User user = (from u in Context.Set<User>()
                         where u.Username.Equals(userName) && u.Password.Equals(password)
                         select u).SingleOrDefault();

            return
                user;
        }
        #endregion
    }
}
