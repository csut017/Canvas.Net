namespace Canvas.Core.Entities;

/// <summary>
/// A rating within a rubric item.
/// </summary>
public record RubricRating
{
    /// <summary>
    /// The long description of the rating.
    /// </summary>
    [JsonPropertyName("long_description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The identifier of the rating.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The maximum number of points possible for this rating.
    /// </summary>
    [JsonPropertyName("points")]
    public double MaxPoints { get; set; }

    /// <summary>
    /// The minimum number of points possible for this rating.
    /// </summary>
    [JsonIgnore]
    public double MinPoints { get; set; }

    /// <summary>
    /// The short description of the rating.
    /// </summary>
    [JsonPropertyName("description")]
    public string Name { get; set; } = string.Empty;
}