namespace Canvas.Core;

/// <summary>
/// An error occurred in the <see cref="IConnection"/>.
/// </summary>
public class ConnectionException
    : Exception
{
    /// <summary>
    /// Initialize a new <see cref="ConnectionException"/> instance.
    /// </summary>
    /// <param name="url">The URL called.</param>
    /// <param name="message">The message.</param>
    public ConnectionException(string url, string? message)
        : this(url, message, null)
    {
    }

    /// <summary>
    /// Initialize a new <see cref="ConnectionException"/> instance.
    /// </summary>
    /// <param name="url">The URL called.</param>
    /// <param name="message">The message.</param>
    /// <param name="innerException">An inner <see cref="Exception"/> instance.</param>
    public ConnectionException(string url, string? message, Exception? innerException)
        : base(message, innerException)
    {
        Url = url;
    }

    /// <summary>
    /// The returned content from the call.
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// The URL called.
    /// </summary>
    public string Url { get; }
}