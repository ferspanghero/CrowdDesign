using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CrowdDesign.Core.Entities;
using CrowdDesign.Infrastructure.SQLServer.Resources;

namespace CrowdDesign.Infrastructure.SQLServer.Repositories
{
    public class ProjectRepository : BaseRepository<Project, int>
    {
        #region Constructors
        public ProjectRepository(DbContext context)
            : base(context)
        { }
        #endregion

        #region Properties
        protected override string EntityNotFoundMessage
        {
            get { return ProjectStrings.ProjectNotFound; }
        }
        #endregion

        #region Methods
        protected override IQueryable<Project> GetRelatedEntities(IQueryable<Project> entitiesQuery)
        {
            return
                entitiesQuery
                    .Include(e => e.Dimensions.Select(d => d.Sketches));
        }

        protected override IEnumerable<Project> ProcessRetrievedEntities(IEnumerable<Project> entities)
        {
            // Unfortunately, Entity Framework does not allow a child collection to be sorted in the Include(...) method. 
            // For example: projectsQuery.Include(d => d.Sketches.OrderBy(s => s.Position).Select(s => s)
            // Since we need to ensure that sketches are ordered by their position inside each dimension, the code below is necessary
            foreach (var project in entities)
                if (project.Dimensions != null)
                    foreach (var dimension in project.Dimensions)
                        if (dimension.Sketches != null)
                            dimension.Sketches = dimension.Sketches.OrderBy(s => s.Position).ToList();

            return
                entities;
        }
        #endregion
    }
}
