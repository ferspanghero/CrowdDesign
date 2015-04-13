using System.Collections.Generic;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces;
using System.Linq;
using System.Data.Entity;

namespace CrowdDesign.Infrastructure.SQLServer
{
    public class ProjectRepository : IProjectRepository
    {
        #region Methods
        public IEnumerable<Project> GetProjects()
        {
            IEnumerable<Project> projects;

            using (var db = new Database())
            {
                // Avoids lazy evaluation issues after the DbContext is disposed by forcing data to be retrieved with IEnumerable.ToList()
                projects = db.Projects
                                .Include(e => e.Categories.Select(u => u.Sketches))
                                .ToList();
            }

            return
                projects;
        }

        public Project GetProject(int projectId)
        {
            Project project;

            using (var db = new Database())
            {
                project = (from p in db.Projects
                           where p.Id == projectId
                           select p)
                           .Include(e => e.Categories.Select(u => u.Sketches))
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
                using (var db = new Database())
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
                using (var db = new Database())
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
            using (var db = new Database())
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

        public int CreateCategory(int projectId, string categoryName)
        {
            int categoryId = -1;

            if (!string.IsNullOrEmpty(categoryName))
            {
                using (var db = new Database())
                {
                    if (db.Projects != null)
                    {
                        Project projectRecord = GetProject(projectId);

                        if (projectRecord != null)
                        {
                            if (projectRecord.Categories == null)
                                projectRecord.Categories = new List<Category>();

                            Category category = new Category
                            {
                                Name = categoryName,
                                Project = projectRecord
                            };

                            // For some reason if we do a db.Table.Add(element), all objects in the graph will be marked as "Added". Therefore, 
                            // the below code attaches the element as in a disconnected scenario and later changes its state to "Added" manually.
                            db.Categories.Attach(category);
                            projectRecord.Categories.Add(category);
                            db.Entry(category).State = EntityState.Added;

                            db.SaveChanges();

                            categoryId = category.Id;
                        }
                    }
                }
            }

            return
                categoryId;
        }

        public Sketch GetSketch(int sketchId)
        {
            Sketch sketch;

            using (var db = new Database())
            {
                sketch = (from s in db.Sketches
                          where s.Id == sketchId
                          select s)
                           .Include(e => e.Category)
                           .Include(e => e.Category.Project)
                           .SingleOrDefault();
            }

            return
                sketch;
        }

        public int CreateSketch(int categoryId)
        {
            int sketchId = -1;

            using (var db = new Database())
            {
                if (db.Projects != null)
                {
                    Category categoryRecord = (from c in db.Categories
                                               where c.Id == categoryId
                                               select c).Include(c => c.Sketches)
                                               .Include(c => c.Project)
                                               .FirstOrDefault();

                    if (categoryRecord != null)
                    {
                        if (categoryRecord.Sketches == null)
                            categoryRecord.Sketches = new List<Sketch>();

                        Sketch sketch = new Sketch
                        {
                            Category = categoryRecord,
                        };

                        // For some reason if we do a db.Table.Add(element), all objects in the graph will be marked as "Added". Therefore, 
                        // the below code attaches the element as in a disconnected scenario and later changes its state to "Added" manually.
                        db.Sketches.Attach(sketch);
                        categoryRecord.Sketches.Add(sketch);
                        db.Entry(sketch).State = EntityState.Added;

                        db.SaveChanges();

                        sketchId = sketch.Id;
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
                using (var db = new Database())
                {
                    if (db.Projects != null)
                    {
                        Sketch sketchRecord = GetSketch(sketch.Id);

                        if (sketchRecord != null)
                        {
                            sketchRecord.Data = sketch.Data;
                            sketchRecord.ImageURI = sketch.ImageURI;
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
