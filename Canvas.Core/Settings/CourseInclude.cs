namespace Canvas.Core.Settings;

/// <summary>
/// Items that can be included when fetching courses.
/// </summary>
[Flags]
public enum CourseInclude
{
    /// <summary>
    /// Do not include anything.
    /// </summary>
    None = 0,

    /// <summary>
    /// Include the term.
    /// </summary>
    Term = 1,

    /// <summary>
    /// Include the allocated teachers.
    /// </summary>
    Teachers = 2,

    /// <summary>
    /// Include the number of students enrolled in the course offering.
    /// </summary>
    TotalStudents = 4,
}