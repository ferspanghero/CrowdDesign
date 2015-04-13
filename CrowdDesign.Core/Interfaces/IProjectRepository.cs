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
        int CreateProject(string projectName);
        void UpdateProject(Project project);
        void DeleteProject(int projectId);        
        int CreateCategory(int projectId, string categoryName);
        Sketch GetSketch(int sketchId);
        int CreateSketch(int categoryId);        
        void UpdateSketch(Sketch sketch);
        #endregion        
    }
}