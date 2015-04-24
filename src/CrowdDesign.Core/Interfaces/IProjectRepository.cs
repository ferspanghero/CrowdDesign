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
        Dimension GetDimension(int dimensionId);
        int CreateDimension(int projectId, Dimension dimension);
        void UpdateDimension(Dimension dimension);
        Sketch GetSketch(int sketchId);
        int CreateSketch(int dimensionId, int userId, Sketch sketch);        
        void UpdateSketch(Sketch sketch);
        #endregion                   
    }
}