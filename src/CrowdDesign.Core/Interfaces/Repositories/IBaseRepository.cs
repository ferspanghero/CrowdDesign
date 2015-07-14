using System;
using System.Collections.Generic;
using CrowdDesign.Core.Interfaces.Entities;

namespace CrowdDesign.Core.Interfaces.Repositories
{
    /// <summary>
    /// Defines a base repository of entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key that uniquely identifies the entity.</typeparam>
    public interface IBaseRepository<TEntity, TKey> : IDisposable
        where TEntity : class, IDomainEntity<TKey>
    {
        #region Methods
        /// <summary>
        /// Gets entities by their id.
        /// </summary>
        /// <param name="entityIds">The ids of the desired entities.</param>
        /// <returns>A collection of entities that match the given ids.</returns>
        /// <remarks>If no ids are provided, all existing entities are returned</remarks>
        IEnumerable<TEntity> Get(params TKey[] entityIds);

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        /// <param name="entity">The new entity to be created.</param>
        /// <returns>The new entity id.</returns>
        TKey Create(TEntity entity);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">The entity to be updated.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entityId">The id of the entity to be deleted.</param>
        void Delete(TKey entityId);
        #endregion
    }
}
