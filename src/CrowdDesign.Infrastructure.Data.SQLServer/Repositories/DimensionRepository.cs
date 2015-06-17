using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces.Repositories;
using CrowdDesign.Infrastructure.SQLServer.Resources;
using CrowdDesign.Utils.Extensions;

namespace CrowdDesign.Infrastructure.SQLServer.Repositories
{
    public class DimensionRepository : BaseRepository<Dimension, int>, IDimensionRepository
    {
        #region Constructors
        public DimensionRepository(DbContext context)
            : base(context)
        { }
        #endregion

        #region Properties
        protected override string EntityNotFoundMessage
        {
            get { return DimensionStrings.DimensionNotFound; }
        }

        protected override string EntityAlreadyExistsMessage
        {
            get { return DimensionStrings.DimensionAlreadyExists; }
        }
        #endregion

        #region Methods
        protected override IQueryable<Dimension> GetRelatedEntities(IQueryable<Dimension> entitiesQuery)
        {
            return
                entitiesQuery
                    .Include(e => e.Sketches)
                    .Include(e => e.Project);
        }

        protected override IEnumerable<Dimension> ProcessRetrievedEntities(IEnumerable<Dimension> entities)
        {
            // Unfortunately, Entity Framework does not allow a child collection to be sorted in the Include(...) method. 
            // For example: projectsQuery.Include(d => d.Sketches.OrderBy(s => s.Position).Select(s => s)
            // Since we need to ensure that sketches are ordered by their position inside each dimension, the code below is necessary
            foreach (var dimension in entities)
                if (dimension.Sketches != null)
                    dimension.Sketches = dimension.Sketches.OrderBy(s => s.Position).ToList();

            return
                entities;
        }

        protected override void CreateEntityRelationships(Dimension entity)
        {
            entity.Project.TryThrowArgumentNullException("dimension.Project");

            Context.Set<Project>().Attach(entity.Project);
        }

        public void MergeDimensions(params int[] dimensionIds)
        {
            dimensionIds.TryThrowArgumentNullException("dimensionIds");

            // The merge algorithm below assumes that the dimensions should be merged to the one whose id is in the last position of the dimensionIds array
            // For example: given an id array [10, 5, 2], [10, 5] will be merged to [2]
            // Consequently, it is necessary to ensure that the dimensions retrieved from the database are ordered exactly like the dimensionIds array
            // The Lookup below is used for this purpose
            var dimensionsLookup = Get(dimensionIds).ToLookup(d => d.Id);
            var dimensionRecords = dimensionIds.SelectMany(id => dimensionsLookup[id]).ToList();

            if (dimensionIds.Length != dimensionRecords.Count)
                throw new InvalidOperationException(DimensionStrings.DimensionsNotFound);

            // If there is more than one dimension being merged
            if (dimensionRecords.Count > 1)
            {
                Dimension targetDimension = dimensionRecords[dimensionRecords.Count - 1];
                int position = targetDimension.Sketches.Count + 1;

                for (int i = 0; i < dimensionRecords.Count - 1; i++)
                {
                    var sketches = dimensionRecords[i].Sketches != null ? new List<Sketch>(dimensionRecords[i].Sketches) : null;

                    if (sketches != null)
                    {
                        foreach (var sketch in sketches)
                        {
                            sketch.Position = position++;
                            sketch.Dimension = targetDimension;

                            targetDimension.Sketches.Add(sketch);
                        }
                    }

                    EntitySet.Remove(dimensionRecords[i]);
                }

                Context.SaveChanges();
            }
        }
        #endregion
    }
}
