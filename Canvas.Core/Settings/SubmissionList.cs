namespace Canvas.Core.Settings;

/// <summary>
/// The settings to use for listing submissions.
/// </summary>
public record SubmissionList
    : List
{
    /// <summary>
    /// The items to include.
    /// </summary>
    public SubmissionInclude Options { get; set; }

    /// <summary>
    /// Implicitly converts an <see cref="SubmissionInclude"/> instance to an <see cref="Options"/> instance.
    /// </summary>
    /// <param name="include">The <see cref="SubmissionInclude"/> instance to convert.</param>
    public static implicit operator SubmissionList(SubmissionInclude include) => new() { Options = include };

    protected override void AppendParameters(Parameters parameters)
    {
        base.AppendParameters(parameters);
        if (Options != SubmissionInclude.None) parameters.Add("include[]", Options);
    }
}