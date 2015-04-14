using System.Collections.Generic;
using CrowdDesign.Core.Entities;

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
        Category GetCategory(int categoryId);
        int CreateCategory(int projectId, Category category);
        void UpdateCategory(Category category);
        Sketch GetSketch(int sketchId);
        int CreateSketch(int categoryId, int userId, Sketch sketch);        
        void UpdateSketch(Sketch sketch);
        #endregion                   
    }
}