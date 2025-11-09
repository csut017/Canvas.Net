namespace Canvas.Core.Settings;

/// <summary>
/// The settings to use for listing assignments.
/// </summary>
public record AssignmentList
    : List
{
    /// <summary>
    /// The items to include.
    /// </summary>
    public AssignmentInclude Options { get; set; }

    /// <summary>
    /// Implicitly converts an <see cref="AssignmentInclude"/> instance to an <see cref="AssignmentList"/> instance.
    /// </summary>
    /// <param name="include">The <see cref="AssignmentInclude"/> instance to convert.</param>
    public static implicit operator AssignmentList(AssignmentInclude include) => new() { Options = include };

    /// <summary>
    /// Appends the parameters to a <see cref="Parameters"/> instance.
    /// </summary>
    /// <param name="parameters">The <see cref="Parameters"/> instance.</param>
    protected override void AppendParameters(Parameters parameters)
    {
        base.AppendParameters(parameters);
        if (Options != AssignmentInclude.None) parameters.Add("include[]", Options);
    }
}