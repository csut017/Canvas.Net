using Serilog;

namespace Canvas.Core;

/// <summary>
/// Defines a connection as using logging.
/// </summary>
public interface ILoggingConnection
    : IConnection
{
    /// <summary>
    /// Updates the logger for the connection.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to use.</param>
    void UpdateLogger(ILogger logger);
}