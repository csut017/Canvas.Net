namespace Canvas.Core.Entities;

/// <summary>
/// A rubric for an assessment submission.
/// </summary>
public record AssessmentRubric
{
    /// <summary>
    /// The associated comments.
    /// </summary>
    public string? Comments { get; init; }

    /// <summary>
    /// The identifier of the associated rubric.
    /// </summary>
    [JsonPropertyName("rating_id")]
    public string? Id { get; init; }

    /// <summary>
    /// The assigned points.
    /// </summary>
    public double? Points { get; init; }
}