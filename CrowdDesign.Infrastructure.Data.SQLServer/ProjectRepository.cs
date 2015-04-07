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
                                .Include(e => e.Categories)
                                .Include(e => e.Users.Select(u => u.Sketches))
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
                           .Include(e => e.Categories)
                           .Include(e => e.Users.Select(u => u.Sketches))                           
                           .SingleOrDefault();
            }

            return
                project;
        }

        public void CreateProject(Project project)
        {
            if (project != null)
            {
                using (var db = new Database())
                {
                    db.Projects.Add(project);
                    db.SaveChanges();
                }
            }
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

        public void CreateCategory(int projectId, string categoryName)
        {
            using (var db = new Database())
            {
                if (db.Projects != null)
                {
                    Project projectRecord = GetProject(projectId);

                    if (projectRecord != null)
                    {
                        using (var transaction = db.Database.BeginTransaction())
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

                            if (projectRecord.Users != null && projectRecord.Users.Count > 0)
                            {
                                foreach (var user in projectRecord.Users)
                                {
                                    if (user.Sketches == null)
                                        user.Sketches = new List<Sketch>();

                                    Sketch sketch = new Sketch
                                    {
                                        Data = null,
                                        User = user,
                                        Category = category
                                    };

                                    db.Sketches.Attach(sketch);
                                    user.Sketches.Add(sketch);
                                    db.Entry(sketch).State = EntityState.Added;
                                }

                                db.SaveChanges();
                            }

                            transaction.Commit();
                        }
                    }
                }
            }
        }

        public void CreateUser(int projectId, string userName)
        {
            using (var db = new Database())
            {
                if (db.Projects != null)
                {
                    Project projectRecord = GetProject(projectId);

                    if (projectRecord != null)
                    {
                        using (var transaction = db.Database.BeginTransaction())
                        {
                            if (projectRecord.Users == null)
                                projectRecord.Users = new List<User>();

                            User user = new User
                            {
                                Name = userName,
                                Project = projectRecord,
                                Sketches = new List<Sketch>()
                            };

                            // For some reason if we do a db.Table.Add(element), all objects in the graph will be marked as "Added". Therefore, 
                            // the below code attaches the element as in a disconnected scenario and later changes its state to "Added" manually.
                            db.Users.Attach(user);
                            projectRecord.Users.Add(user);
                            db.Entry(user).State = EntityState.Added;

                            db.SaveChanges();

                            if (projectRecord.Categories != null)
                            {
                                foreach (var category in projectRecord.Categories)
                                {
                                    Sketch sketch = new Sketch
                                    {
                                        Data = null,
                                        User = user,
                                        Category = category
                                    };

                                    db.Sketches.Attach(sketch);
                                    user.Sketches.Add(sketch);
                                    db.Entry(sketch).State = EntityState.Added;
                                }

                                db.SaveChanges();
                            }

                            transaction.Commit();
                        }
                    }
                }
            }
        }

        public Sketch GetSketch(int sketchId)
        {
            Sketch sketch;

            using (var db = new Database())
            {
                sketch = (from s in db.Sketches
                           where s.Id == sketchId
                           select s).SingleOrDefault();
            }

            return
                sketch;
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
