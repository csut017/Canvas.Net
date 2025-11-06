namespace Canvas.Core.Http;

/// <summary>
/// Extensions for configuring HTTP connections.
/// </summary>
public static class ConfigurationExtensions
{
    extension(ClientConfiguration config)
    {
        /// <summary>
        /// Connects to Canvas via HTTP.
        /// </summary>
        /// <param name="url">The URL to the Canvas server.</param>
        /// <param name="token">The default user token to use.</param>
        /// <returns>The <see href="ClientConfiguration"/> instance.</returns>
        public ClientConfiguration ViaHttp(
            string url,
            string token)
        {
            var conn = new Connection(url, token);
            config.Connection = conn;
            return config;
        }
    }
}