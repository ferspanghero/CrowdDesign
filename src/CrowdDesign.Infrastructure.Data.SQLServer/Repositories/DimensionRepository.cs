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
    public class DimensionRepository : IDimensionRepository
    {
        #region Methods
        public IEnumerable<Dimension> GetDimensions(params int[] dimensionIds)
        {
            IEnumerable<Dimension> dimensions;

            using (var db = new DatabaseContext())
            {
                IQueryable<Dimension> dimensionsQuery;

                // This if block is necessary because Entity Framework does not support checking if a collection is null or empty 
                // inside a LINQ query. This happens because it cannot convert this kind of query to SQL code               
                if (dimensionIds == null || dimensionIds.Length == 0)
                {
                    dimensionsQuery = from e in db.Dimensions
                                      select e;
                }
                else
                {
                    dimensionsQuery = from e in db.Dimensions
                                      where dimensionIds.Contains(e.Id)
                                      select e;
                }

                // Avoids lazy evaluation issues after the DbContext is disposed by forcing data to be retrieved with IEnumerable.ToList()
                dimensions = dimensionsQuery
                                .Include(e => e.Sketches)
                                .Include(e => e.Project)
                                .ToList();

                // Unfortunately, Entity Framework does not allow a child collection to be sorted in the Include(...) method. 
                // For example: projectsQuery.Include(d => d.Sketches.OrderBy(s => s.Position).Select(s => s)
                // Since we need to ensure that sketches are ordered by their position inside each dimension, the code below is necessary
                foreach (var dimension in dimensions)
                    if (dimension.Sketches != null)
                        dimension.Sketches = dimension.Sketches.OrderBy(s => s.Position).ToList();
            }

            return
                dimensions;
        }

        public int CreateDimension(Dimension dimension)
        {
            int dimensionId = -1;

            dimension.TryThrowArgumentNullException("dimension");
            dimension.Project.TryThrowArgumentNullException("dimension.Project");

            using (var db = new DatabaseContext())
            {
                if (db.Projects != null)
                {
                    // TODO: These dependencies should be injected
                    IProjectRepository projectRepository = new ProjectRepository();

                    Project projectRecord = projectRepository.GetProjects(dimension.Project.Id).SingleOrDefault();

                    if (projectRecord == null)
                        throw new InvalidOperationException(ProjectStrings.ProjectNotFound);

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

            return
                dimensionId;
        }

        public void UpdateDimension(Dimension dimension)
        {
            dimension.TryThrowArgumentNullException("dimension");

            using (var db = new DatabaseContext())
            {
                Dimension dimensionRecord = GetDimensions(dimension.Id).SingleOrDefault();

                if (dimensionRecord == null)
                    throw new InvalidOperationException(DimensionStrings.DimensionNotFound);

                dimensionRecord.Name = dimension.Name;
                dimensionRecord.Description = dimension.Description;
                dimensionRecord.SortCriteria = dimension.SortCriteria;
                db.Entry(dimensionRecord).State = EntityState.Modified;

                db.SaveChanges();
            }
        }

        public void DeleteDimension(int dimensionId)
        {
            using (var db = new DatabaseContext())
            {
                Dimension dimensionRecord = GetDimensions(dimensionId).SingleOrDefault();

                if (dimensionRecord == null)
                    throw new InvalidOperationException(DimensionStrings.DimensionNotFound);

                db.Entry(dimensionRecord).State = EntityState.Deleted;

                db.SaveChanges();
            }
        }

        public void MergeDimensions(params int[] dimensionIds)
        {
            dimensionIds.TryThrowArgumentNullException("dimensionIds");

            using (var db = new DatabaseContext())
            {
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

                        db.Dimensions.Attach(dimensionRecords[i]);
                        db.Dimensions.Remove(dimensionRecords[i]);

                        if (sketches != null)
                        {
                            foreach (var sketch in sketches)
                            {
                                db.Entry(sketch).State = EntityState.Modified;

                                sketch.Position = position++;
                                sketch.Dimension = targetDimension;

                                targetDimension.Sketches.Add(sketch);
                            }
                        }
                    }

                    db.SaveChanges();
                }
            }
        }
        #endregion
    }
}
