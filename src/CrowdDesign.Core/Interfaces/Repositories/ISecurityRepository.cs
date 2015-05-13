using CrowdDesign.Core.Entities;

namespace CrowdDesign.Core.Interfaces.Repositories
{
    /// <summary>
    /// Defines a repository of all data related to the system's security.
    /// </summary>
    public interface ISecurityRepository : IBaseRepository<User, int>
    {
        #region Methods
        /// <summary>
        /// Logs a user into the system.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>The logged user.</returns>
        User Login(string userName, string password);
        #endregion
    }
}