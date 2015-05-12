using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces;
using CrowdDesign.Infrastructure.SQLServer.Resources;
using CrowdDesign.Utils.Extensions;

namespace CrowdDesign.Infrastructure.SQLServer.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        #region Constructors
        public ProjectRepository(DbContext context)
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
        public IEnumerable<Project> GetProjects(params int[] projectIds)
        {
            IQueryable<Project> projectsQuery;

            // This if block is necessary because Entity Framework does not support checking if a collection is null or empty 
            // inside a LINQ query. This happens because it cannot convert this kind of query to SQL code               
            if (projectIds == null || projectIds.Length == 0)
            {
                projectsQuery = from e in _context.Set<Project>()
                                select e;
            }
            else
            {
                projectsQuery = from e in _context.Set<Project>()
                                where projectIds.Contains(e.Id)
                                select e;
            }

            // Avoids lazy evaluation issues after the DbContext is disposed by forcing data to be retrieved with IEnumerable.ToList()
            IEnumerable<Project> projects = projectsQuery
                                    .Include(e => e.Dimensions.Select(d => d.Sketches))
                                    .ToList();

            // Unfortunately, Entity Framework does not allow a child collection to be sorted in the Include(...) method. 
            // For example: projectsQuery.Include(d => d.Sketches.OrderBy(s => s.Position).Select(s => s)
            // Since we need to ensure that sketches are ordered by their position inside each dimension, the code below is necessary
            foreach (var project in projects)
                if (project.Dimensions != null)
                    foreach (var dimension in project.Dimensions)
                        if (dimension.Sketches != null)
                            dimension.Sketches = dimension.Sketches.OrderBy(s => s.Position).ToList();

            return
                projects;
        }

        public int CreateProject(Project project)
        {
            int projectId = -1;

            project.TryThrowArgumentNullException("project");

            _context.Set<Project>().Add(project);
            _context.SaveChanges();

            projectId = project.Id;

            return
                projectId;
        }

        public void UpdateProject(Project project)
        {
            project.TryThrowArgumentNullException("project");

            Project projectRecord = GetProjects(project.Id).SingleOrDefault();

            if (projectRecord == null)
                throw new InvalidOperationException(ProjectStrings.ProjectNotFound);

            projectRecord.Name = project.Name;

            _context.SaveChanges();
        }

        public void DeleteProject(int projectId)
        {
            Project projectRecord = GetProjects(projectId).SingleOrDefault();

            if (projectRecord == null)
                throw new InvalidOperationException(ProjectStrings.ProjectNotFound);

            _context.Set<Project>().Remove(projectRecord);

            _context.SaveChanges();
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
