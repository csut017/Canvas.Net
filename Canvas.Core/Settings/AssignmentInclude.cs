namespace Canvas.Core.Settings;

/// <summary>
/// Items that can be included when fetching assignments.
/// </summary>
[Flags]
public enum AssignmentInclude
{
    /// <summary>
    /// Do not include anything.
    /// </summary>
    None = 0,

    /// <summary>
    /// Include all the assignment dates.
    /// </summary>
    AllDates = 1,
}