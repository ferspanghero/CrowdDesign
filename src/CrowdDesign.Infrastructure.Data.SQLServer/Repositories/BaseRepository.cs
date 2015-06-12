using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CrowdDesign.Core.Interfaces.Entities;
using CrowdDesign.Core.Interfaces.Repositories;
using CrowdDesign.Utils.Extensions;

namespace CrowdDesign.Infrastructure.SQLServer.Repositories
{
    public abstract class BaseRepository<TEntity, TKey> : IBaseRepository<TEntity, TKey>
        where TEntity : class, IDomainEntity<TKey>
    {
        #region Constructors
        public BaseRepository(DbContext context)
        {
            context.TryThrowArgumentNullException("context");
            Context = context;

            EntitySet = Context.Set<TEntity>();
            EntitySet.TryThrowArgumentNullException("context.Set<TEntity>");

            _disposed = false;
        }
        #endregion

        #region Fields
        private bool _disposed;
        #endregion

        #region Properties
        #region Abstract
        /// <summary>
        /// Gets a message to be displayed when the entity is not found.
        /// </summary>
        protected abstract string EntityNotFoundMessage { get; }
        #endregion

        /// <summary>
        /// Gets or sets the Entity Framework database context.
        /// </summary>
        protected DbContext Context { get; private set; }

        /// <summary>
        /// Gets or sets a collection that represents the entity database table.
        /// </summary>
        protected DbSet<TEntity> EntitySet { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Includes in the main entities retrieved from the database their related entities.
        /// </summary>
        /// <param name="entitiesQuery">The query that contains the main entities.</param>
        /// <returns>A query that includes all related entities.</returns>
        protected virtual IQueryable<TEntity> GetRelatedEntities(IQueryable<TEntity> entitiesQuery)
        {
            return
                entitiesQuery;
        }

        /// <summary>
        /// Processes the entities retrieved from the database with additional operations like, for instance, filtering and/or sorting.
        /// </summary>
        /// <param name="entities">The retrieved entities.</param>
        /// <returns>The processed entities.</returns>
        protected virtual IEnumerable<TEntity> ProcessRetrievedEntities(IEnumerable<TEntity> entities)
        {
            return
                entities;
        }

        /// <summary>
        /// Creates for a new main entity the relationships with its related entities.
        /// </summary>
        /// <param name="entity">The new main entity.</param>
        protected virtual void CreateEntityRelationships(TEntity entity)
        {
            return;
        }

        public IEnumerable<TEntity> Get(params TKey[] entityIds)
        {
            IQueryable<TEntity> entitiesQuery;

            // This if block is necessary because Entity Framework does not support checking if a collection is null or empty 
            // inside a LINQ query. This happens because it cannot convert this kind of query to SQL code               
            if (entityIds == null || entityIds.Length == 0)
            {
                entitiesQuery = from e in EntitySet
                                select e;
            }
            else
            {
                entitiesQuery = from e in EntitySet
                                where entityIds.Contains(e.Id)
                                select e;
            }

            // Avoids lazy evaluation issues after the DbContext is disposed by forcing data to be retrieved with IEnumerable.ToList()
            IEnumerable<TEntity> entities = GetRelatedEntities(entitiesQuery).ToList();

            entities = ProcessRetrievedEntities(entities);

            return
                entities;
        }        

        public TKey Create(TEntity entity)
        {
            entity.TryThrowArgumentNullException("entity");

            CreateEntityRelationships(entity);

            EntitySet.Add(entity);
            Context.SaveChanges();

            TKey entityId = entity.Id;

            return
                entityId;
        }

        public void Update(TEntity entity)
        {
            entity.TryThrowArgumentNullException("entity");

            EntitySet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;

            Context.SaveChanges();
        }

        public void Delete(TKey entityId)
        {
            TEntity entityRecord = EntitySet.Find(entityId);

            if (entityRecord == null)
                throw new InvalidOperationException(EntityNotFoundMessage);

            EntitySet.Remove(entityRecord);

            Context.SaveChanges();
        }

        public bool AnyEntity(Func<TEntity, bool> pred)
        {
            return 
                EntitySet.Any(pred);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Context.Dispose();
                _disposed = true;
            }
        }
        #endregion
    }
}
