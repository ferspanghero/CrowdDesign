using System.Data.Entity;
using System.Linq;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces;
using CrowdDesign.Infrastructure.SQLServer.Contexts;

namespace CrowdDesign.Infrastructure.SQLServer.Repositories
{
    public class SecurityRepository : ISecurityRepository
    {
        #region Methods
        public User Login(string userName, string password)
        {
            User user;

            using (var db = new DatabaseContext())
            {
                user = (from u in db.Users
                        where u.Username.Equals(userName) && u.Password.Equals(password)
                        select u).SingleOrDefault();
            }

            return
                user;
        }

        public User GetUser(int userId)
        {
            User user;

            using (var db = new DatabaseContext())
            {
                user = (from u in db.Users
                        where u.Id == userId
                        select u).SingleOrDefault();
            }

            return
                user;
        }
        #endregion
    }
}
