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
    public class DimensionRepository : IDimensionRepository
    {
        #region Constructors
        public DimensionRepository(DbContext context)
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
        public IEnumerable<Dimension> GetDimensions(params int[] dimensionIds)
        {
            IQueryable<Dimension> dimensionsQuery;

            // This if block is necessary because Entity Framework does not support checking if a collection is null or empty 
            // inside a LINQ query. This happens because it cannot convert this kind of query to SQL code               
            if (dimensionIds == null || dimensionIds.Length == 0)
            {
                dimensionsQuery = from e in _context.Set<Dimension>()
                                  select e;
            }
            else
            {
                dimensionsQuery = from e in _context.Set<Dimension>()
                                  where dimensionIds.Contains(e.Id)
                                  select e;
            }

            // Avoids lazy evaluation issues after the DbContext is disposed by forcing data to be retrieved with IEnumerable.ToList()
            IEnumerable<Dimension> dimensions = dimensionsQuery
                                        .Include(e => e.Sketches)
                                        .Include(e => e.Project)
                                        .ToList();

            // Unfortunately, Entity Framework does not allow a child collection to be sorted in the Include(...) method. 
            // For example: projectsQuery.Include(d => d.Sketches.OrderBy(s => s.Position).Select(s => s)
            // Since we need to ensure that sketches are ordered by their position inside each dimension, the code below is necessary
            foreach (var dimension in dimensions)
                if (dimension.Sketches != null)
                    dimension.Sketches = dimension.Sketches.OrderBy(s => s.Position).ToList();

            return
                dimensions;
        }

        public int CreateDimension(Dimension dimension)
        {
            dimension.TryThrowArgumentNullException("dimension");
            dimension.Project.TryThrowArgumentNullException("dimension.Project");

            // TODO: These dependencies should be injected
            IProjectRepository projectRepository = new ProjectRepository(_context);

            Project projectRecord = projectRepository.GetProjects(dimension.Project.Id).SingleOrDefault();

            if (projectRecord == null)
                throw new InvalidOperationException(ProjectStrings.ProjectNotFound);

            if (projectRecord.Dimensions == null)
                projectRecord.Dimensions = new List<Dimension>();

            dimension.Project = projectRecord;

            projectRecord.Dimensions.Add(dimension);

            _context.SaveChanges();

            int dimensionId = dimension.Id;

            return
                dimensionId;
        }

        public void UpdateDimension(Dimension dimension)
        {
            dimension.TryThrowArgumentNullException("dimension");

            Dimension dimensionRecord = GetDimensions(dimension.Id).SingleOrDefault();

            if (dimensionRecord == null)
                throw new InvalidOperationException(DimensionStrings.DimensionNotFound);

            dimensionRecord.Name = dimension.Name;
            dimensionRecord.Description = dimension.Description;
            dimensionRecord.SortCriteria = dimension.SortCriteria;

            _context.SaveChanges();
        }

        public void DeleteDimension(int dimensionId)
        {
            Dimension dimensionRecord = GetDimensions(dimensionId).SingleOrDefault();

            if (dimensionRecord == null)
                throw new InvalidOperationException(DimensionStrings.DimensionNotFound);

            _context.Set<Dimension>().Remove(dimensionRecord);

            _context.SaveChanges();
        }

        public void MergeDimensions(params int[] dimensionIds)
        {
            dimensionIds.TryThrowArgumentNullException("dimensionIds");

            // The merge algorithm below assumes that the dimensions should be merged to the one whose id is in the last position of the dimensionIds array
            // For example: given an id array [10, 5, 2], [10, 5] will be merged to [2]
            // Consequently, it is necessary to ensure that the dimensions retrieved from the database are ordered exactly like the dimensionIds array
            // The Lookup below is used for this purpose
            var dimensionsLookup = GetDimensions(dimensionIds).ToLookup(d => d.Id);
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

                    _context.Set<Dimension>().Remove(dimensionRecords[i]);
                }

                _context.SaveChanges();
            }
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
