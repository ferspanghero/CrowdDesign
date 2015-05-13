namespace CrowdDesign.Core.Interfaces.Entities
{
    /// <summary>
    /// Defines the core behavior of a domain entity.
    /// </summary>
    /// <typeparam name="TKey">The type of the key that uniquely identifies the entity.</typeparam>
    public interface IDomainEntity<TKey>
    {
        /// <summary>
        /// Gets or sets a key that uniquely identifies the entity.
        /// </summary>
        TKey Id { get; set; }
    }
}
