namespace Canvas.Core.Internal;

/// <summary>
/// Allows configuring a connection.
/// </summary>
internal class ClientConnectionConfiguration(ClientConfiguration configuration)
    : IClientConnectionConfiguration
{
    /// <summary>
    /// The underlying connection.
    /// </summary>
    public IConnection? Connection { get; private set; }

    /// <summary>
    /// The <see cref="ClientConfiguration"/> that owns this <see cref="IClientConnectionConfiguration"/>.
    /// </summary>
    public ClientConfiguration Owner => configuration;

    /// <summary>
    /// Updates the connection.
    /// </summary>
    /// <param name="connection"></param>
    public void Connect(IConnection connection)
    {
        Connection = connection;
    }
}