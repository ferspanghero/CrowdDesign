using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces;
using CrowdDesign.Infrastructure.SQLServer.Contexts;
using CrowdDesign.Utils.Extensions;

namespace CrowdDesign.Infrastructure.SQLServer.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        #region Methods
        #region Projects

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
                    throw new InvalidOperationException(Resources.ProjectStrings.ProjectNotFound);

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
                    throw new InvalidOperationException(Resources.ProjectStrings.ProjectNotFound);

                db.Entry(projectRecord).State = EntityState.Deleted;

                db.SaveChanges();

            }
        }

        #endregion

        #region Dimensions

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
                    Project projectRecord = GetProjects(dimension.Project.Id).SingleOrDefault();

                    if (projectRecord == null)
                        throw new InvalidOperationException(Resources.ProjectStrings.ProjectNotFound);

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
                    throw new InvalidOperationException(Resources.ProjectStrings.DimensionNotFound);

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
                    throw new InvalidOperationException(Resources.ProjectStrings.DimensionNotFound);

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
                    throw new InvalidOperationException(Resources.ProjectStrings.DimensionsNotFound);

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

        #region Sketches
        public IEnumerable<Sketch> GetSketches(params int[] sketchIds)
        {
            IEnumerable<Sketch> sketches;

            using (var db = new DatabaseContext())
            {
                IQueryable<Sketch> sketchesQuery;

                // This if block is necessary because Entity Framework does not support checking if a collection is null or empty 
                // inside a LINQ query. This happens because it cannot convert this kind of query to SQL code               
                if (sketchIds == null || sketchIds.Length == 0)
                {
                    sketchesQuery = from e in db.Sketches
                                    orderby e.Position
                                    select e;
                }
                else
                {
                    sketchesQuery = from e in db.Sketches
                                    where sketchIds.Contains(e.Id)
                                    orderby e.Position
                                    select e;
                }

                // Avoids lazy evaluation issues after the DbContext is disposed by forcing data to be retrieved with IEnumerable.ToList()
                sketches = sketchesQuery
                                .Include(e => e.Dimension)
                                .Include(e => e.Dimension.Project)
                                .Include(e => e.Dimension.Sketches)
                                .Include(e => e.User)
                                .ToList();
            }

            return
                sketches;
        }

        public int CreateSketch(Sketch sketch)
        {
            int sketchId = -1;

            sketch.TryThrowArgumentNullException("sketch");
            sketch.User.TryThrowArgumentNullException("sketch.User");
            sketch.Dimension.TryThrowArgumentNullException("sketch.Dimension");

            using (var db = new DatabaseContext())
            {
                // TODO: This dependency should be injected
                ISecurityRepository securityRepository = new SecurityRepository();

                Dimension dimensionRecord = GetDimensions(sketch.Dimension.Id).SingleOrDefault();
                User userRecord = securityRepository.GetUser(sketch.User.Id);

                if (dimensionRecord == null)
                    throw new InvalidOperationException(Resources.ProjectStrings.DimensionNotFound);

                if (userRecord == null)
                    throw new InvalidOperationException(Resources.SecurityStrings.UserNotFound);

                if (dimensionRecord.Sketches == null)
                    dimensionRecord.Sketches = new List<Sketch>();

                sketch.Dimension = dimensionRecord;
                sketch.User = userRecord;

                // For some reason if we do a db.Table.Add(element), all objects in the graph will be marked as "Added". Therefore, 
                // the below code attaches the element as in a disconnected scenario and later changes its state to "Added" manually.
                db.Sketches.Attach(sketch);
                dimensionRecord.Sketches.Add(sketch);
                db.Entry(sketch).State = EntityState.Added;

                db.SaveChanges();

                sketchId = sketch.Id;
            }

            return
                sketchId;
        }

        public void UpdateSketch(Sketch sketch)
        {
            sketch.TryThrowArgumentNullException("sketch");
            sketch.Dimension.TryThrowArgumentNullException("sketch.Dimension");

            using (var db = new DatabaseContext())
            {
                Sketch sketchRecord = GetSketches(sketch.Id).SingleOrDefault();

                if (sketchRecord == null)
                    throw new InvalidOperationException(Resources.ProjectStrings.SketchNotFound);

                sketchRecord.Data = sketch.Data;
                sketchRecord.ImageUri = sketch.ImageUri;
                sketchRecord.Position = sketch.Position;

                if (sketch.Dimension != null && sketch.Dimension.Id != sketchRecord.Dimension.Id)
                {
                    sketchRecord.Dimension = sketch.Dimension;

                    // TODO: This is definitely not the most efficient way to change a child's element parent. Best would be to do it 
                    // within one EF context and avoid using a TransactionScope. However, as of now, I was not able to make EF 
                    // update the relationships correctly within one context
                    DeleteSketch(sketchRecord.Id);
                    CreateSketch(sketchRecord);
                }
                else
                {
                    db.Entry(sketchRecord).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public void DeleteSketch(int sketchId)
        {
            using (var db = new DatabaseContext())
            {
                Sketch sketchRecord = GetSketches(sketchId).SingleOrDefault();

                if (sketchRecord == null)
                    throw new InvalidOperationException(Resources.ProjectStrings.SketchNotFound);

                db.Entry(sketchRecord).State = EntityState.Deleted;

                db.SaveChanges();
            }
        }

        public void ReplaceSketches(int sourceSketchId, int targetSketchId)
        {
            using (var db = new DatabaseContext())
            {
                var sketches = GetSketches(sourceSketchId, targetSketchId).ToDictionary(s => s.Id, s => s);

                if (sketches.Count != 2)
                    throw new InvalidOperationException(Resources.ProjectStrings.SketchesNotFound);

                Sketch sourceSketch = sketches[sourceSketchId];
                Sketch targetSketch = sketches[targetSketchId];

                using (var transaction = new TransactionScope())
                {
                    // CASE 1: If the sketch is being moved within its current dimension
                    if (sourceSketch.Dimension.Id == targetSketch.Dimension.Id)
                    {
                        int targetSketchOriginalPosition = targetSketch.Position;

                        // CASE 1.1: If the sketch is being moved to a superior position
                        if (sourceSketch.Position < targetSketch.Position)
                        {
                            foreach (var sketch in sourceSketch.Dimension.Sketches)
                            {
                                // Decrements the position of sketches that belong to the moved sketch dimension
                                // For example: for sketches in positions [1, 2, 3, 4, 5], if [2] is moved to the position of [4], 
                                //              resulting in [1, 3, 4, 4, 5], then [3, 4] need to be decremented                                
                                if (sketch.Position > sourceSketch.Position &&
                                    sketch.Position <= targetSketch.Position)
                                {
                                    sketch.Position--;
                                    db.Entry(sketch).State = EntityState.Modified;
                                }
                            }
                        }
                        // CASE 1.2: If the sketch is being moved to an inferior position
                        else if (sourceSketch.Position > targetSketch.Position)
                        {
                            foreach (var sketch in sourceSketch.Dimension.Sketches)
                            {
                                // Increments the position of sketches that belong to the moved sketch dimension
                                // For example: for sketches in positions [1, 2, 3, 4, 5], if [4] is moved to the position of [2], 
                                //              resulting in [1, 2, 2, 3, 5], then [2, 3] need to be incremented
                                if (sketch.Position >= targetSketch.Position &&
                                    sketch.Position < sourceSketch.Position)
                                {
                                    sketch.Position++;
                                    db.Entry(sketch).State = EntityState.Modified;
                                }
                            }
                        }

                        // Assigns to the moved sketch the position of the target sketch
                        sourceSketch.Position = targetSketchOriginalPosition;

                        db.Entry(sourceSketch).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    // CASE 2: If the sketch is being moved to a new dimension
                    else
                    {
                        // Decrements the position of all sketches that belong to the moved sketch original dimension that were in a position after it
                        // For example: for sketches in positions [1, 2, 3, 4], if [2] is moved, then [3, 4] need to be updated
                        //              the updated positions will then be [1, 2, 3]
                        foreach (var sketch in sourceSketch.Dimension.Sketches)
                        {
                            if (sketch.Position > sourceSketch.Position)
                            {
                                sketch.Position--;
                                db.Entry(sketch).State = EntityState.Modified;
                            }
                        }

                        // Assigns to the moved sketch the position of the target sketch
                        sourceSketch.Position = targetSketch.Position;

                        // Increments the position of all sketches that belong to this new dimension 
                        // that will be in a position after it
                        // For example: for sketches in positions [1, 2, 3, 4], if the sketch is moved to the position where is [2], then [2, 3, 4] need to be updated
                        //              the updated positions will then be [1, 2, 3, 4, 5], where [2] is the moved sketch
                        foreach (var sketch in targetSketch.Dimension.Sketches)
                        {
                            if (sketch.Position >= sourceSketch.Position)
                            {
                                sketch.Position++;
                                db.Entry(sketch).State = EntityState.Modified;
                            }
                        }

                        db.SaveChanges();

                        // Assigns to the moved sketch the dimension of the target sketch
                        sourceSketch.Dimension = targetSketch.Dimension;

                        UpdateSketch(sourceSketch);
                    }

                    transaction.Complete();
                }
            }
        }

        public void MoveSketchToDimension(int sourceSketchId, int targetDimensionId)
        {
            using (var db = new DatabaseContext())
            {
                Sketch sourceSketch = GetSketches(sourceSketchId).SingleOrDefault();                

                if (sourceSketch == null)
                    throw new InvalidOperationException(Resources.ProjectStrings.SketchNotFound);                

                // The general idea of this algorithm is that if a sketch is moved directly to a dimension,
                // it will be placed in the end position
                // For example: if a sketch [5] is moved to the dimension with sketches [1, 2, 3, 4], then
                //              it will become [1, 2, 3, 4, 5]

                using (var transaction = new TransactionScope())
                {
                    // CASE 1: If the sketch is being moved to its current dimension
                    if (sourceSketch.Dimension.Id == targetDimensionId)
                    {
                        foreach (var sketch in sourceSketch.Dimension.Sketches)
                        {
                            // Decrements the position of sketches that belong to the moved sketch dimension
                            // For example: for sketches in positions [1, 2, 3, 4, 5], if [2] is moved,
                            //              resulting in [1, 3, 4, 5, 5], then [3, 4, 5] need to be decremented                                
                            if (sketch.Position > sourceSketch.Position && sketch.Position <= sourceSketch.Dimension.Sketches.Count)
                            {
                                sketch.Position--;
                                db.Entry(sketch).State = EntityState.Modified;
                            }
                        }

                        sourceSketch.Position = sourceSketch.Dimension.Sketches.Count == 0 ? 1 : sourceSketch.Dimension.Sketches.Count;

                        db.Entry(sourceSketch).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    // CASE 2: If the sketch is being moved to a new dimension
                    else
                    {
                        Dimension targetDimension = GetDimensions(targetDimensionId).SingleOrDefault();

                        if (targetDimension == null)
                            throw new InvalidOperationException(Resources.ProjectStrings.DimensionNotFound);

                        // Decrements the position of all sketches that belong to the moved sketch original dimension that were in a position after it
                        // For example: for sketches in positions [1, 2, 3, 4], if [2] is moved, then [3, 4] need to be updated
                        //              the updated positions will then be [1, 2, 3]
                        foreach (var sketch in sourceSketch.Dimension.Sketches)
                        {
                            if (sketch.Position > sourceSketch.Position)
                            {
                                sketch.Position--;
                                db.Entry(sketch).State = EntityState.Modified;
                            }
                        }

                        // Assigns to the moved sketch the end position of the target dimension
                        sourceSketch.Position = targetDimension.Sketches.Count == 0 ? 1 : targetDimension.Sketches.Count;

                        db.SaveChanges();

                        // Assigns to the moved sketch the dimension of the target sketch
                        sourceSketch.Dimension = targetDimension;

                        UpdateSketch(sourceSketch);
                    }

                    transaction.Complete();
                }               
            }
        }
        #endregion
        #endregion
    }
}
