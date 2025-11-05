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
    public string? Content { get; set; }

    /// <summary>
    /// The URL called.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// Generates a new <see cref="ConnectionException"/> from an <see cref="HttpRequestMessage"/> instance.
    /// </summary>
    /// <param name="url">The URL called.</param>
    /// <param name="response">The <see cref="HttpResponseMessage"/> instance.</param>
    /// <returns>A new <see cref="ConnectionException"/> instance.</returns>
    public static ConnectionException New(string url, HttpResponseMessage? response)
    {
        if (response == null) return new ConnectionException(url, "Something went wrong while calling Canvas");

        var exception = new ConnectionException(url, $"Canvas returned a non-success response code [{response.StatusCode}]");
        using var stream = response.Content.ReadAsStream();
        using var reader = new StreamReader(stream);
        exception.Content = reader.ReadToEnd();
        return exception;
    }
}