namespace Canvas.Core.Entities;

/// <summary>
/// Ae entity with an identifier.
/// </summary>
/// <remarks>
/// This interface is required, as some JSON structures for Canvas use a different attribute name for
/// the id property.
/// </remarks>
public interface IEntityWithId
{
    /// <summary>
    /// The identifier of the entity.
    /// </summary>
    ulong Id { get; }
}