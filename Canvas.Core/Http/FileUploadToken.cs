namespace Canvas.Core.Http;

/// <summary>
/// The upload token returned from Canvas.
/// </summary>
public record FileUploadToken
{
    /// <summary>
    /// The parameters to pass.
    /// </summary>
    [JsonPropertyName("upload_params")]
    public Dictionary<string, string> Parameters { get; init; } = [];

    /// <summary>
    /// The upload URL.
    /// </summary>
    [JsonPropertyName("upload_url")]
    public string? Url { get; init; }
}