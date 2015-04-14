using System.Collections.Generic;
using CrowdDesign.Core.Entities;

namespace CrowdDesign.Core.Interfaces
{
    public interface ISecurityRepository
    {
        #region Methods
        User Login(string userName, string password);
        User GetUser(int userId);
        #endregion                                     
    }
}