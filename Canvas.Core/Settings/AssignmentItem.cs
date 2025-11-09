namespace Canvas.Core.Settings;

/// <summary>
/// The options for retrieving an assignment.
/// </summary>
public record AssignmentItem
    : ISettings
{
    /// <summary>
    /// The items to include.
    /// </summary>
    public AssignmentInclude Options { get; set; }

    /// <summary>
    /// Implicitly converts an <see cref="AssignmentInclude"/> instance to an <see cref="AssignmentItem"/> instance.
    /// </summary>
    /// <param name="include">The <see cref="AssignmentInclude"/> instance to convert.</param>
    public static implicit operator AssignmentItem(AssignmentInclude include) => new() { Options = include };

    /// <summary>
    /// Generates a set of parameters with the current values from these settings.
    /// </summary>
    /// <returns>The new <see cref="Parameters"/> instance containing the settings.</returns>
    public Parameters ToParameters()
    {
        var parameters = new Parameters();
        if (Options != AssignmentInclude.None) parameters.Add("include[]", Options);
        return parameters;
    }
}