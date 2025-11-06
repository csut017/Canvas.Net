using Canvas.Core.Entities;

namespace Canvas.Core.Settings;

/// <summary>
/// The settings to use for listing courses.
/// </summary>
public record CourseList
    : List
{
    /// <summary>
    /// Filter by an enrolment type.
    /// </summary>
    public EnrolmentType? EnrolmentType { get; init; }

    /// <summary>
    /// The options to include.
    /// </summary>
    public CourseInclude Options { get; init; }

    /// <summary>
    /// Filter by a term identifier.
    /// </summary>
    public ulong? TermId { get; init; }

    /// <summary>
    /// Implicitly converts an <see cref="CourseInclude"/> instance to an <see cref="Options"/> instance.
    /// </summary>
    /// <param name="include">The <see cref="CourseInclude"/> instance to convert.</param>
    public static implicit operator CourseList(CourseInclude include) => new() { Options = include };

    /// <summary>
    /// Appends the parameters to a <see cref="Parameters"/> instance.
    /// </summary>
    /// <param name="parameters">The <see cref="Parameters"/> instance.</param>
    protected override void AppendParameters(Parameters parameters)
    {
        base.AppendParameters(parameters);
        if (EnrolmentType != null) parameters.Add("enrollment_type", EnrolmentType.Value);
        if (Options != CourseInclude.None) parameters.Add("include[]", Options);
        if (TermId != null) parameters.Add("enrollment_term_id", TermId.Value);
    }
}