using CrowdDesign.Core.Entities;
using System;
using System.Collections.Generic;

namespace CrowdDesign.Core.Interfaces
{
    public interface IProjectRepository
    {
        #region Methods
        IEnumerable<Project> GetProjects();
        Project GetProject(int projectId);
        void CreateProject(Project project);
        void UpdateProject(Project project);
        void DeleteProject(int projectId);
        void CreateCategory(int projectId, string categoryName);
        void CreateUser(int projectId, string userName);
        Sketch GetSketch(int sketchId);
        void UpdateSketch(Sketch sketch);
        #endregion        
    }
}