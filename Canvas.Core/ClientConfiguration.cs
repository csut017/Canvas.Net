namespace Canvas.Core;

/// <summary>
/// Configuration object for creating <see cref="IClient"/> instances.
/// </summary>
public class ClientConfiguration
{
    /// <summary>
    /// The underlying connection to use.
    /// </summary>
    public IConnection? Connection { get; set; }

    /// <summary>
    /// Builds an <see cref="IClient"/> instance from the configuration settings.
    /// </summary>
    /// <returns>A new <see cref="IClient"/> instance.</returns>
    public IClient Build()
    {
        if (Connection == null) throw new ConfigurationException("Connection must be initialised.");

        return new Client();
    }
}