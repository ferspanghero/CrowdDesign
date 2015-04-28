using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces;
using CrowdDesign.Infrastructure.SQLServer.Contexts;

namespace CrowdDesign.Infrastructure.SQLServer.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        #region Methods
        public IEnumerable<Project> GetProjects()
        {
            IEnumerable<Project> projects;

            using (var db = new DatabaseContext())
            {
                // Avoids lazy evaluation issues after the DbContext is disposed by forcing data to be retrieved with IEnumerable.ToList()
                projects = db.Projects
                                .Include(e => e.Dimensions.Select(u => u.Sketches))
                                .ToList();
            }

            return
                projects;            
        }

        public Project GetProject(int projectId)
        {
            Project project;

            using (var db = new DatabaseContext())
            {
                project = (from p in db.Projects
                           where p.Id == projectId
                           select p)
                           .Include(e => e.Dimensions.Select(u => u.Sketches.Select(s => s.User)))
                           .SingleOrDefault();
            }

            return
                project;
        }

        public int CreateProject(string projectName)
        {
            int projectId = -1;

            if (!string.IsNullOrEmpty(projectName))
            {
                using (var db = new DatabaseContext())
                {
                    Project project = new Project { Name = projectName };

                    db.Projects.Add(project);
                    db.SaveChanges();

                    projectId = project.Id;
                }
            }

            return
                projectId;
        }

        public void UpdateProject(Project project)
        {
            if (project != null)
            {
                using (var db = new DatabaseContext())
                {
                    if (db.Projects != null)
                    {
                        Project projectRecord = GetProject(project.Id);

                        if (projectRecord != null)
                        {
                            projectRecord.Name = project.Name;
                            db.Entry(projectRecord).State = EntityState.Modified;

                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        public void DeleteProject(int projectId)
        {
            using (var db = new DatabaseContext())
            {
                if (db.Projects != null)
                {
                    Project projectRecord = GetProject(projectId);

                    if (projectRecord != null)
                        db.Entry(projectRecord).State = EntityState.Deleted;

                    db.SaveChanges();
                }
            }
        }

        public Dimension GetDimension(int dimensionId)
        {
            Dimension dimension;

            using (var db = new DatabaseContext())
            {
                dimension = (from c in db.Dimensions
                            where c.Id == dimensionId
                            select c).Include(c => c.Sketches)
                            .Include(c => c.Project)
                            .FirstOrDefault();
            }

            return
                dimension;
        }

        public int CreateDimension(int projectId, Dimension dimension)
        {
            int dimensionId = -1;

            if (dimension != null)
            {
                using (var db = new DatabaseContext())
                {
                    if (db.Projects != null)
                    {
                        Project projectRecord = GetProject(projectId);

                        if (projectRecord != null)
                        {
                            if (projectRecord.Dimensions == null)
                                projectRecord.Dimensions = new List<Dimension>();

                            dimension.Project = projectRecord;

                            // For some reason if we do a db.Table.Add(element), all objects in the graph will be marked as "Added". Therefore, 
                            // the below code attaches the element as in a disconnected scenario and later changes its state to "Added" manually.
                            db.Dimensions.Attach(dimension);
                            projectRecord.Dimensions.Add(dimension);
                            db.Entry(dimension).State = EntityState.Added;

                            db.SaveChanges();

                            dimensionId = dimension.Id;
                        }
                    }
                }
            }

            return
                dimensionId;
        }

        public void UpdateDimension(Dimension dimension)
        {
            if (dimension != null)
            {
                using (var db = new DatabaseContext())
                {
                    if (db.Projects != null)
                    {
                        Dimension dimensionRecord = GetDimension(dimension.Id);

                        if (dimensionRecord != null)
                        {
                            dimensionRecord.Name = dimension.Name;
                            dimensionRecord.Description = dimension.Description;
                            dimensionRecord.SortCriteria = dimension.SortCriteria;
                            db.Entry(dimensionRecord).State = EntityState.Modified;

                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        public Sketch GetSketch(int sketchId)
        {
            Sketch sketch;

            using (var db = new DatabaseContext())
            {
                sketch = (from s in db.Sketches
                          where s.Id == sketchId
                          select s)
                           .Include(e => e.Dimension)
                           .Include(e => e.Dimension.Project)
                           .Include(e => e.User)
                           .SingleOrDefault();
            }

            return
                sketch;
        }

        public int CreateSketch(int dimensionId, int userId, Sketch sketch)
        {
            int sketchId = -1;

            if (sketch != null)
            {
                using (var dbProject = new DatabaseContext())
                {
                    if (dbProject.Projects != null)
                    {
                        ISecurityRepository securityRepository = new SecurityRepository();

                        Dimension dimensionRecord = GetDimension(dimensionId);
                        User userRecord = securityRepository.GetUser(userId);

                        if (dimensionRecord != null && userRecord != null)
                        {
                            if (dimensionRecord.Sketches == null)
                                dimensionRecord.Sketches = new List<Sketch>();

                            sketch.Dimension = dimensionRecord;
                            sketch.User = userRecord;

                            // For some reason if we do a db.Table.Add(element), all objects in the graph will be marked as "Added". Therefore, 
                            // the below code attaches the element as in a disconnected scenario and later changes its state to "Added" manually.
                            dbProject.Sketches.Attach(sketch);
                            dimensionRecord.Sketches.Add(sketch);
                            dbProject.Entry(sketch).State = EntityState.Added;

                            dbProject.SaveChanges();

                            sketchId = sketch.Id;
                        }
                    }
                }
            }

            return
                sketchId;
        }

        public void UpdateSketch(Sketch sketch)
        {
            if (sketch != null)
            {
                using (var db = new DatabaseContext())
                {
                    if (db.Projects != null)
                    {
                        Sketch sketchRecord = GetSketch(sketch.Id);

                        if (sketchRecord != null)
                        {
                            sketchRecord.Data = sketch.Data;
                            sketchRecord.ImageUri = sketch.ImageUri;
                            db.Entry(sketchRecord).State = EntityState.Modified;

                            db.SaveChanges();
                        }
                    }
                }
            }
        }
        #endregion
    }
}
