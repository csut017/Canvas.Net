namespace Canvas.Core;

/// <summary>
/// An error occurred in the <see cref="IClient"/> or associated interfaces.
/// </summary>
public class ClientException
    : Exception
{
    /// <summary>
    /// Initialize a new <see cref="ClientException"/> instance.
    /// </summary>
    /// <param name="message">The message.</param>
    public ClientException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initialize a new <see cref="ClientException"/> instance.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">An inner <see cref="Exception"/> instance.</param>
    public ClientException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}