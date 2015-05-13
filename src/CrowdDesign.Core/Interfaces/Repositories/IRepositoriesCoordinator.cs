using System;

namespace CrowdDesign.Core.Interfaces.Repositories
{
    /// <summary>
    /// Represents a coordinator responsible for managing the data repositories
    /// </summary>
    /// <remarks>
    /// The coordinator employes the Unit of Work pattern 
    /// (http://www.asp.net/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application)
    /// </remarks>
    public interface IRepositoriesCoordinator : IDisposable
    {
    }
}
