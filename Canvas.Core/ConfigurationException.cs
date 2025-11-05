namespace Canvas.Core;

/// <summary>
/// An exception that occurs when the configuration is checked.
/// </summary>
[Serializable]
public class ConfigurationException : Exception
{
    /// <summary>
    /// Initialise a new <see cref="ConfigurationException"/> with a message.
    /// </summary>
    /// <param name="message">The message.</param>
    public ConfigurationException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initialise a new <see cref="ConfigurationException"/> with a message and an inner exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ConfigurationException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}