using Canvas.Core.Entities;

namespace Canvas.Core.Settings;

/// <summary>
/// The settings to use for retrieving a course.
/// </summary>
public record CourseItem
    : ISettings
{
    /// <summary>
    /// Initialise a new <see cref="CourseItem"/> instance.
    /// </summary>
    /// <param name="enrolmentTypes">The enrolment types to filter by.</param>
    public CourseItem(params EnrolmentType[] enrolmentTypes)
    {
        EnrolmentTypes = [.. enrolmentTypes];
    }

    /// <summary>
    /// Initialise a new <see cref="CourseItem"/> instance.
    /// </summary>
    public CourseItem()
        : this([])
    {
    }

    /// <summary>
    /// The enrolment types to retrieve.
    /// </summary>
    /// <remarks>
    /// This parameter will be ignored if empty.
    /// </remarks>
    public List<EnrolmentType> EnrolmentTypes { get; }

    /// <summary>
    /// The options to include.
    /// </summary>
    public CourseInclude Options { get; init; }

    /// <summary>
    /// Implicitly converts an <see cref="CourseInclude"/> instance to an <see cref="CourseItem"/> instance.
    /// </summary>
    /// <param name="include">The <see cref="CourseInclude"/> instance to convert.</param>
    public static implicit operator CourseItem(CourseInclude include) => new() { Options = include };

    /// <summary>
    /// Generates a set of parameters with the current values from these settings.
    /// </summary>
    /// <returns>The new <see cref="Parameters"/> instance containing the settings.</returns>
    public Parameters ToParameters()
    {
        var parameters = new Parameters();
        if (Options != CourseInclude.None) parameters.Add("include[]", Options);
        if (EnrolmentTypes.Count <= 0) return parameters;

        foreach (var type in EnrolmentTypes)
        {
            parameters.Add("enrollment_type", type.ToString().ToLowerInvariant());
        }
        return parameters;
    }
}