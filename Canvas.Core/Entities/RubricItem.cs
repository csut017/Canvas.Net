namespace Canvas.Core.Entities;

/// <summary>
/// A rubric item that can be used for an assignment.
/// </summary>
public record RubricItem
{
    /// <summary>
    /// The description of the rubric.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The identifier of the rubric.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// A flag indicating whether the rubric is a range.
    /// </summary>
    [JsonPropertyName("criterion_use_range")]
    public bool IsRange { get; set; }

    /// <summary>
    /// The ratings within the rubric.
    /// </summary>
    public List<RubricRating>? Ratings { get; set; }
}