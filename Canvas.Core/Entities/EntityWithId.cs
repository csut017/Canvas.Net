using System.Diagnostics;

namespace Canvas.Core.Entities;

/// <summary>
/// An entity with an identifier.
/// </summary>
/// <remarks>
/// This record is the base for most entities in Canvas. It provides some core functionality with working
/// with entities based on their identifier.
/// </remarks>
[DebuggerDisplay($"{{{nameof(Id)}}}")]
public abstract record EntityWithId
    : IEntityWithId
{
    /// <summary>
    /// The identifier of the entity.
    /// </summary>
    public ulong Id { get; init; }

    /// <summary>
    /// Returns the hash code of the identifier.
    /// </summary>
    /// <returns>The hashcode from the identifier.</returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    /// <summary>
    /// An <see cref="IEqualityComparer{EntityWithId}"/> comparer.
    /// </summary>
    public class EquityComparer
        : IEqualityComparer<EntityWithId>
    {
        /// <summary>
        /// Compares two <see cref="EntityWithId"/> instances to check if they are the same.
        /// </summary>
        /// <param name="x">The first <see cref="EntityWithId"/> instance.</param>
        /// <param name="y">The second <see cref="EntityWithId"/> instance.</param>
        /// <returns><c>true</c> if the identifiers are the same; <c>false</c> otherwise.</returns>
        public bool Equals(EntityWithId? x, EntityWithId? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id == y.Id;
        }

        /// <summary>
        /// Returns the hash code of the identifier.
        /// </summary>
        /// <returns>The hashcode from the identifier.</returns>
        public int GetHashCode(EntityWithId obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}