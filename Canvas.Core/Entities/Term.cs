namespace Canvas.Core.Entities;

/// <summary>
/// A term in Canvas.
/// </summary>
public record Term
    : EntityWithIdAndName
{
    /// <summary>
    /// The date the term was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime? WhenCreated { get; init; }

    /// <summary>
    /// The date the term ends (or ended).
    /// </summary>
    [JsonPropertyName("end_at")]
    public DateTime? WhenEnds { get; init; }

    /// <summary>
    /// The date the term starts (or started).
    /// </summary>
    [JsonPropertyName("start_at")]
    public DateTime? WhenStart { get; init; }
}