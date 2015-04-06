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
                            projectRecord.Categories = project.Categories;
                            projectRecord.Users = project.Users;

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
                        db.Projects.Remove(projectRecord);

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
                                ProjectId = projectRecord.Id,
                                Project = projectRecord
                            };

                            projectRecord.Categories.Add(category);

                            db.SaveChanges();

                            if (projectRecord.Users != null)
                            {
                                foreach (var user in projectRecord.Users)
                                {
                                    if (user.Sketches == null)
                                        user.Sketches = new List<Sketch>();

                                    user.Sketches.Add(new Sketch
                                    {
                                        Image = null,
                                        UserId = user.Id,
                                        User = user,
                                        CategoryId = category.Id,
                                        Category = category
                                    });
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
                                ProjectId = projectRecord.Id,
                                Project = projectRecord,
                                Sketches = new List<Sketch>()
                            };

                            projectRecord.Users.Add(user);

                            db.SaveChanges();

                            if (projectRecord.Categories != null)
                            {
                                foreach (var category in projectRecord.Categories)
                                {
                                    user.Sketches.Add(new Sketch
                                    {
                                        Image = null,
                                        UserId = user.Id,
                                        User = user,
                                        CategoryId = category.Id,
                                        Category = category
                                    });
                                }

                                db.SaveChanges();
                            }

                            transaction.Commit();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
