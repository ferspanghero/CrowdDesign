using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces;
using CrowdDesign.Infrastructure.SQLServer.Contexts;
using CrowdDesign.Infrastructure.SQLServer.Resources;
using CrowdDesign.Utils.Extensions;

namespace CrowdDesign.Infrastructure.SQLServer.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        #region Methods
        public IEnumerable<Project> GetProjects(params int[] projectIds)
        {
            IEnumerable<Project> projects;

            using (var db = new DatabaseContext())
            {
                IQueryable<Project> projectsQuery;

                // This if block is necessary because Entity Framework does not support checking if a collection is null or empty 
                // inside a LINQ query. This happens because it cannot convert this kind of query to SQL code               
                if (projectIds == null || projectIds.Length == 0)
                {
                    projectsQuery = from e in db.Projects
                                    select e;
                }
                else
                {
                    projectsQuery = from e in db.Projects
                                    where projectIds.Contains(e.Id)
                                    select e;
                }

                // Avoids lazy evaluation issues after the DbContext is disposed by forcing data to be retrieved with IEnumerable.ToList()
                projects = projectsQuery
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
            }

            return
                projects;
        }

        public int CreateProject(Project project)
        {
            int projectId = -1;

            project.TryThrowArgumentNullException("project");

            using (var db = new DatabaseContext())
            {
                db.Projects.Add(project);
                db.SaveChanges();

                projectId = project.Id;
            }

            return
                projectId;
        }

        public void UpdateProject(Project project)
        {
            project.TryThrowArgumentNullException("project");

            using (var db = new DatabaseContext())
            {
                Project projectRecord = GetProjects(project.Id).SingleOrDefault();

                if (projectRecord == null)
                    throw new InvalidOperationException(ProjectStrings.ProjectNotFound);

                projectRecord.Name = project.Name;
                db.Entry(projectRecord).State = EntityState.Modified;

                db.SaveChanges();
            }
        }

        public void DeleteProject(int projectId)
        {
            using (var db = new DatabaseContext())
            {
                Project projectRecord = GetProjects(projectId).SingleOrDefault();

                if (projectRecord == null)
                    throw new InvalidOperationException(ProjectStrings.ProjectNotFound);

                db.Entry(projectRecord).State = EntityState.Deleted;

                db.SaveChanges();

            }
        }
        #endregion
    }
}
