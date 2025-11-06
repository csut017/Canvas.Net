namespace Canvas.Core;

/// <summary>
/// Defines the interface for setting a <see cref="IConnection"/> to a client.
/// </summary>
public interface IClientConnectionConfiguration
{
    /// <summary>
    /// The <see cref="ClientConfiguration"/> that owns this <see cref="IClientConnectionConfiguration"/>.
    /// </summary>
    ClientConfiguration Owner { get; }

    /// <summary>
    /// Updates the connection.
    /// </summary>
    /// <param name="connection"></param>
    void Connect(IConnection connection);
}