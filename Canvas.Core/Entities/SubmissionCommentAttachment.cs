namespace Canvas.Core.Entities;

/// <summary>
/// An attachment for a submission comment.
/// </summary>
public record SubmissionCommentAttachment
    : EntityWithId
{
    /// <summary>
    /// The identifier of the author.
    /// </summary>
    public ulong AuthorId { get; init; }

    /// <summary>
    /// The content type of the attachment.
    /// </summary>
    [JsonPropertyName("content-type")]
    public string? ContentType { get; init; }

    /// <summary>
    /// The display name of the attachment.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; init; }

    /// <summary>
    /// The filename of the attachment.
    /// </summary>
    public string? Filename { get; init; }

    /// <summary>
    /// The URL to download the attachment from.
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// The date and time when the attachment was added.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTime? WhenAdded { get; init; }
}