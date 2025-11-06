using Serilog;

namespace Canvas.Core;

/// <summary>
/// Configuration object for creating <see cref="IClient"/> instances.
/// </summary>
public class ClientConfiguration
{
    /// <summary>
    /// The <see cref="IConnection"/> instance to use.
    /// </summary>
    public IConnection? Connection { get; set; }

    /// <summary>
    /// The <see cref="ILogger"/> instance to use.
    /// </summary>
    public ILogger? Logger { get; set; }

    /// <summary>
    /// Builds an <see cref="IClient"/> instance from the configuration settings.
    /// </summary>
    /// <returns>A new <see cref="IClient"/> instance.</returns>
    public IClient Build()
    {
        if (Connection == null) throw new ConfigurationException("Connection must be initialised.");

        Logger?.Information("Building client");
        return new Client(Connection, Logger);
    }

    /// <summary>
    /// Specifies the logger to use.
    /// </summary>
    /// <param name="logger">A <see cref="ILogger"/> instance.</param>
    /// <returns>The <see cref="ClientConfiguration"/> instance.</returns>
    public ClientConfiguration UseLogger(ILogger logger)
    {
        this.Logger = logger;
        return this;
    }
}