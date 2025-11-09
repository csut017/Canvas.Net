namespace Canvas.Core.Entities;

/// <summary>
/// A peer review for an assignment.
/// </summary>
public record PeerReview
    : EntityWithId
{
    /// <summary>
    /// The identifier of the assessor.
    /// </summary>
    [JsonPropertyName("assessor_id")]
    public ulong AssessorId { get; init; }

    /// <summary>
    /// The identifier of the user who submitted the assignment.
    /// </summary>
    [JsonPropertyName("user_id")]
    public ulong UserId { get; init; }
}