namespace Canvas.Core;

/// <summary>
/// The settings to use when listing data from the server.
/// </summary>
public record ListSettings
{
    /// <summary>
    /// The maximum number of pages.
    /// </summary>
    public int MaxPages { get; init; } = int.MaxValue;
}