using System.Diagnostics;

namespace Canvas.Core.Entities;

/// <summary>
/// An entity with an identifier.
/// </summary>
[DebuggerDisplay($"{{{nameof(Name)}}} [{{{nameof(Id)}}}]")]
public record EntityWithIdAndName
    : EntityWithId, IComparable, IComparable<EntityWithIdAndName>
{
    /// <summary>
    /// The name of the entity.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Compares this entity with another entity based on their names.
    /// </summary>
    /// <param name="obj">The other entity to check.</param>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
    /// <list type="table">
    /// <listheader><term> Value</term><description> Meaning</description></listheader>
    /// <item><term> Less than zero</term><description> This instance precedes <paramref name="obj" /> in the sort order.</description></item>
    /// <item><term> Zero</term><description> This instance occurs in the same position in the sort order as <paramref name="obj" />.</description></item>
    /// <item><term> Greater than zero</term><description> This instance follows <paramref name="obj" /> in the sort order.</description></item></list>
    /// </returns>
    public int CompareTo(object? obj)
    {
        return CompareTo(obj as EntityWithIdAndName);
    }

    /// <summary>
    /// Compares this entity with another entity based on their names.
    /// </summary>
    /// <param name="other">The other <see cref="EntityWithIdAndName"/> instance to check.</param>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
    /// <list type="table">
    /// <listheader><term> Value</term><description> Meaning</description></listheader>
    /// <item><term> Less than zero</term><description> This instance precedes <paramref name="other" /> in the sort order.</description></item>
    /// <item><term> Zero</term><description> This instance occurs in the same position in the sort order as <paramref name="other" />.</description></item>
    /// <item><term> Greater than zero</term><description> This instance follows <paramref name="other" /> in the sort order.</description></item></list>
    /// </returns>
    public int CompareTo(EntityWithIdAndName? other)
    {
        return string.CompareOrdinal(Name, other?.Name);
    }
}