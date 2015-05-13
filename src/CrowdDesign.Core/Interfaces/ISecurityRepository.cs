using System;
using CrowdDesign.Core.Entities;

namespace CrowdDesign.Core.Interfaces
{
    /// <summary>
    /// Defines a repository of all data related to the system's security.
    /// </summary>
    public interface ISecurityRepository : IDisposable
    {
        #region Methods
        /// <summary>
        /// Logs a user into the system.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>The logged user.</returns>
        User Login(string userName, string password);

        /// <summary>
        /// Gets a user by its id.
        /// </summary>
        /// <param name="userId">The id of the desired user.</param>
        /// <returns>The user that matches the given id.</returns>
        User GetUser(int userId);
        #endregion
    }
}