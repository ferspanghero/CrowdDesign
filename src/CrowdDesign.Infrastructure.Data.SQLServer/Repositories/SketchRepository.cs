using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CrowdDesign.Core.Entities;
using CrowdDesign.Core.Interfaces;
using CrowdDesign.Infrastructure.SQLServer.Contexts;
using CrowdDesign.Infrastructure.SQLServer.Resources;
using CrowdDesign.Utils.Extensions;

namespace CrowdDesign.Infrastructure.SQLServer.Repositories
{
    public class SketchRepository : ISketchRepository
    {
        #region Constructors
        public SketchRepository(DbContext context)
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
        public IEnumerable<Sketch> GetSketches(params int[] sketchIds)
        {
            IQueryable<Sketch> sketchesQuery;

            // This if block is necessary because Entity Framework does not support checking if a collection is null or empty 
            // inside a LINQ query. This happens because it cannot convert this kind of query to SQL code               
            if (sketchIds == null || sketchIds.Length == 0)
            {
                sketchesQuery = from e in _context.Set<Sketch>()
                                orderby e.Position
                                select e;
            }
            else
            {
                sketchesQuery = from e in _context.Set<Sketch>()
                                where sketchIds.Contains(e.Id)
                                orderby e.Position
                                select e;
            }

            // Avoids lazy evaluation issues after the DbContext is disposed by forcing data to be retrieved with IEnumerable.ToList()
            IEnumerable<Sketch> sketches = sketchesQuery
                                    .Include(e => e.Dimension)
                                    .Include(e => e.Dimension.Project)
                                    .Include(e => e.Dimension.Sketches)
                                    .Include(e => e.User)
                                    .ToList();

            return
                sketches;
        }

        public int CreateSketch(Sketch sketch)
        {
            sketch.TryThrowArgumentNullException("sketch");
            sketch.User.TryThrowArgumentNullException("sketch.User");
            sketch.Dimension.TryThrowArgumentNullException("sketch.Dimension");

            // TODO: These dependencies should be injected
            ISecurityRepository securityRepository = new SecurityRepository(_context);
            IDimensionRepository dimensionRepository = new DimensionRepository(_context);

            Dimension dimensionRecord = dimensionRepository.GetDimensions(sketch.Dimension.Id).SingleOrDefault();
            User userRecord = securityRepository.GetUser(sketch.User.Id);

            if (dimensionRecord == null)
                throw new InvalidOperationException(DimensionStrings.DimensionNotFound);

            if (userRecord == null)
                throw new InvalidOperationException(SecurityStrings.UserNotFound);

            if (dimensionRecord.Sketches == null)
                dimensionRecord.Sketches = new List<Sketch>();

            sketch.Dimension = dimensionRecord;
            sketch.User = userRecord;

            dimensionRecord.Sketches.Add(sketch);

            _context.SaveChanges();

            int sketchId = sketch.Id;

            return
                sketchId;
        }

        public void UpdateSketch(Sketch sketch)
        {
            sketch.TryThrowArgumentNullException("sketch");
            sketch.Dimension.TryThrowArgumentNullException("sketch.Dimension");

            Sketch sketchRecord = GetSketches(sketch.Id).SingleOrDefault();

            if (sketchRecord == null)
                throw new InvalidOperationException(SketchStrings.SketchNotFound);

            sketchRecord.Data = sketch.Data;
            sketchRecord.ImageUri = sketch.ImageUri;
            sketchRecord.Position = sketch.Position;

            _context.SaveChanges();
        }

        public void DeleteSketch(int sketchId)
        {
            using (var db = new DatabaseContext())
            {
                Sketch sketchRecord = GetSketches(sketchId).SingleOrDefault();

                if (sketchRecord == null)
                    throw new InvalidOperationException(SketchStrings.SketchNotFound);

                _context.Set<Sketch>().Remove(sketchRecord);

                db.SaveChanges();
            }
        }

        public void ReplaceSketches(int sourceSketchId, int targetSketchId)
        {
            var sketches = GetSketches(sourceSketchId, targetSketchId).ToDictionary(s => s.Id, s => s);

            if (sketches.Count != 2)
                throw new InvalidOperationException(SketchStrings.SketchesNotFound);

            Sketch sourceSketch = sketches[sourceSketchId];
            Sketch targetSketch = sketches[targetSketchId];

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
                        if (sketch.Position > sourceSketch.Position && sketch.Position <= targetSketch.Position)
                            sketch.Position--;
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
                        if (sketch.Position >= targetSketch.Position && sketch.Position < sourceSketch.Position)
                            sketch.Position++;
                    }
                }

                // Assigns to the moved sketch the position of the target sketch
                sourceSketch.Position = targetSketchOriginalPosition;
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
                        sketch.Position--;
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
                        sketch.Position++;
                }

                // Assigns to the moved sketch the dimension of the target sketch
                sourceSketch.Dimension = targetSketch.Dimension;
            }

            _context.SaveChanges();
        }

        public void MoveSketchToDimension(int sourceSketchId, int targetDimensionId)
        {
            Sketch sourceSketch = GetSketches(sourceSketchId).SingleOrDefault();

            if (sourceSketch == null)
                throw new InvalidOperationException(SketchStrings.SketchNotFound);

            // The general idea of this algorithm is that if a sketch is moved directly to a dimension,
            // it will be placed in the end position
            // For example: if a sketch [5] is moved to the dimension with sketches [1, 2, 3, 4], then
            //              it will become [1, 2, 3, 4, 5]

            // CASE 1: If the sketch is being moved to its current dimension
            if (sourceSketch.Dimension.Id == targetDimensionId)
            {
                foreach (var sketch in sourceSketch.Dimension.Sketches)
                {
                    // Decrements the position of sketches that belong to the moved sketch dimension
                    // For example: for sketches in positions [1, 2, 3, 4, 5], if [2] is moved,
                    //              resulting in [1, 3, 4, 5, 5], then [3, 4, 5] need to be decremented                                
                    if (sketch.Position > sourceSketch.Position && sketch.Position <= sourceSketch.Dimension.Sketches.Count)
                        sketch.Position--;
                }

                sourceSketch.Position = sourceSketch.Dimension.Sketches.Count == 0 ? 1 : sourceSketch.Dimension.Sketches.Count;
            }
            // CASE 2: If the sketch is being moved to a new dimension
            else
            {
                // TODO: These dependencies should be injected
                IDimensionRepository dimensionRepository = new DimensionRepository(_context);

                Dimension targetDimension = dimensionRepository.GetDimensions(targetDimensionId).SingleOrDefault();

                if (targetDimension == null)
                    throw new InvalidOperationException(DimensionStrings.DimensionNotFound);

                // Decrements the position of all sketches that belong to the moved sketch original dimension that were in a position after it
                // For example: for sketches in positions [1, 2, 3, 4], if [2] is moved, then [3, 4] need to be updated
                //              the updated positions will then be [1, 2, 3]
                foreach (var sketch in sourceSketch.Dimension.Sketches)
                {
                    if (sketch.Position > sourceSketch.Position)
                        sketch.Position--;
                }

                // Assigns to the moved sketch the end position of the target dimension
                sourceSketch.Position = targetDimension.Sketches.Count == 0 ? 1 : targetDimension.Sketches.Count;

                // Assigns to the moved sketch the dimension of the target sketch
                sourceSketch.Dimension = targetDimension;
            }

            _context.SaveChanges();
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
