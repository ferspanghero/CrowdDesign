using System.Data.Entity;
using System.Linq;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces;
using CrowdDesign.Utils.Extensions;

namespace CrowdDesign.Infrastructure.SQLServer.Repositories
{
    public class SecurityRepository : ISecurityRepository
    {
        #region Constructors
        public SecurityRepository(DbContext context)
        {
            context.TryThrowArgumentNullException("context");

            _context = context;
            _disposed = false;
        }
        #endregion

        #region Fields
        private readonly DbContext _context;
        private bool _disposed;
        #endregion

        #region Methods
        public User Login(string userName, string password)
        {
            User user = (from u in _context.Set<User>()
                         where u.Username.Equals(userName) && u.Password.Equals(password)
                         select u).SingleOrDefault();

            return
                user;
        }

        public User GetUser(int userId)
        {
            User user = (from u in _context.Set<User>()
                         where u.Id == userId
                         select u).SingleOrDefault();

            return
                user;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _context.Dispose();
                _disposed = true;
            }
        }
        #endregion
    }
}
