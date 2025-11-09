namespace Canvas.Core.Entities;

/// <summary>
/// A comment on a submission.
/// </summary>
public record SubmissionComment
    : EntityWithId
{
    /// <summary>
    /// The attached files for the submission.
    /// </summary>
    public List<SubmissionCommentAttachment>? Attachments { get; init; }

    /// <summary>
    /// The author of the comment.
    /// </summary>
    [JsonPropertyName("author_name")]
    public string? Author { get; init; }

    /// <summary>
    /// The identifier of the author.
    /// </summary>
    [JsonPropertyName("author_id")]
    public ulong AuthorId { get; init; }

    /// <summary>
    /// The text of the comment.
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Text { get; init; }

    /// <summary>
    /// The date and time when the comment was added.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime? WhenAdded { get; init; }
}