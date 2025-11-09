namespace Canvas.Core.Entities;

/// <summary>
/// A file included in a submission.
/// </summary>
public record SubmissionFile
{
    /// <summary>
    /// The name of the file.
    /// </summary>
    public string Filename { get; init; } = string.Empty;

    /// <summary>
    /// A URL to the file on Canvas.
    /// </summary>
    public string Url { get; init; } = string.Empty;

    /// <summary>
    /// The date and time when the file was uploaded.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTime? WhenUploaded { get; init; }
}