using Canvas.Core.Internal;

namespace Canvas.Core;

/// <summary>
/// Configuration object for creating <see cref="IClient"/> instances.
/// </summary>
public class ClientConfiguration
{
    private readonly ClientConnectionConfiguration _connectionConfiguration;

    /// <summary>
    /// Initialise a new <see cref="ClientConfiguration"/> instance.
    /// </summary>
    public ClientConfiguration()
    {
        _connectionConfiguration = new ClientConnectionConfiguration(this);
    }

    /// <summary>
    /// The underlying connection to use.
    /// </summary>
    public IClientConnectionConfiguration Connection => _connectionConfiguration;

    /// <summary>
    /// Builds an <see cref="IClient"/> instance from the configuration settings.
    /// </summary>
    /// <returns>A new <see cref="IClient"/> instance.</returns>
    public IClient Build()
    {
        if (_connectionConfiguration.Connection == null) throw new ConfigurationException("Connection must be initialised.");

        return new Client();
    }
}