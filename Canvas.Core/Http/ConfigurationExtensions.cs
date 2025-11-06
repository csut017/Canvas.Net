using Serilog;

namespace Canvas.Core.Http;

/// <summary>
/// Extensions for configuring HTTP connections.
/// </summary>
public static class ConfigurationExtensions
{
    extension(IClientConnectionConfiguration config)
    {
        /// <summary>
        /// Connects to Canvas via HTTP.
        /// </summary>
        /// <param name="url">The URL to the Canvas server.</param>
        /// <param name="token">The default user token to use.</param>
        /// <param name="logger">An optional <see cref="ILogger"/> to use for any logging.</param>
        /// <returns>The owner of the <see cref="IClientConnectionConfiguration"/> instance.</returns>
        public ClientConfiguration ViaHttp(
            string url,
            string token,
            ILogger? logger = null)
        {
            var conn = new Connection(url, token, logger);
            config.Connect(conn);
            return config.Owner;
        }
    }
}