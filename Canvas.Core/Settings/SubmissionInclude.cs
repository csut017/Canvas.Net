namespace Canvas.Core.Settings;

/// <summary>
/// Items that can be included when fetching assignment submissions.
/// </summary>
[Flags]
public enum SubmissionInclude
{
    /// <summary>
    /// Do not include anything.
    /// </summary>
    None = 0,

    /// <summary>
    /// Include the user.
    /// </summary>
    User = 1,

    /// <summary>
    /// Include any comments.
    /// </summary>
    SubmissionComments = 2,

    /// <summary>
    /// Include any rubric details.
    /// </summary>
    RubricAssessment = 4,

    /// <summary>
    /// Include all details
    /// </summary>
    All = 255,
}