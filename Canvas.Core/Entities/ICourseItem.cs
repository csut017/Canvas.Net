namespace Canvas.Core.Entities;

/// <summary>
/// Defines an entity as belonging to a course.
/// </summary>
public interface ICourseItem
{
    /// <summary>
    /// The identifier of the owning course.
    /// </summary>
    ulong CourseId { get; }
}