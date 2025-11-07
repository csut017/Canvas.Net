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

        /// <summary>
        /// Connects to Canvas via HTTP and logs any responses.
        /// </summary>
        /// <param name="url">The URL to the Canvas server.</param>
        /// <param name="token">The default user token to use.</param>
        /// <returns>The <see href="ClientConfiguration"/> instance.</returns>
        public ClientConfiguration ViaHttpWithResponseLogging(
            string url,
            string token)
        {
            var conn = new Connection(url, token)
            {
                InitialiseResponseStream = LogIncomingContent
            };
            config.Connection = conn;
            return config;
        }
    }

    /// <summary>
    /// Helper method to log all incoming content.
    /// </summary>
    /// <param name="conn">The associated connection.</param>
    /// <param name="resp">The <see cref="HttpResponseMessage"/> instance containing the stream.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel the operation.</param>
    /// <returns>A <see cref="Stream"/> containing the response data.</returns>
    public static async Task<Stream> LogIncomingContent(Connection conn, HttpResponseMessage resp,
        CancellationToken cancellationToken)
    {
        if (!string.Equals("application/json", resp.Content.Headers.ContentType?.MediaType))
        {
            conn.Logger?.Debug("Received non-JSON data");
            return await resp.Content.ReadAsStreamAsync(cancellationToken);
        }

        var stream = new MemoryStream();
        await resp.Content.CopyToAsync(stream, cancellationToken);
        stream.Seek(0, SeekOrigin.Begin);
        using (var reader = new StreamReader(stream, leaveOpen: true))
        {
            var content = await reader.ReadToEndAsync(cancellationToken);
            conn.Logger?.Debug("Received: {content}", content);
        }

        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }
}