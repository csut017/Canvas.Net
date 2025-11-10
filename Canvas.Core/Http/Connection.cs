using Canvas.Core.Settings;
using CommunityToolkit.Diagnostics;
using Serilog;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Canvas.Core.Http;

/// <summary>
/// A <see cref="IConnection"/> instance that connects to a Canvas server using HTTP.
/// </summary>
public class Connection
    : ILoggingConnection
{
    private readonly HttpClient _client;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>
    /// Initialize a new <see cref="Connection"/> instance using a URL and token.
    /// </summary>
    /// <param name="url">The URL to the Canvas server.</param>
    /// <param name="token">The default user token to use.</param>
    /// <param name="client">An optional <see cref="HttpClient"/> instance.</param>
    public Connection(
        string url,
        string token,
        HttpClient? client = null)
    {
        Guard.IsNotNullOrWhiteSpace(url);
        Guard.IsNotNullOrWhiteSpace(token);

        _client = client ?? new HttpClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Do some basic cleaning operations - it won't change everything, but it will catch the obvious ones
        var parts = url.Split('/');
        var length = parts.Length - 1;
        if (string.IsNullOrEmpty(parts[length])) length--;
        if (string.Equals(parts[length], "v1", StringComparison.InvariantCultureIgnoreCase)) length--;
        if (string.Equals(parts[length], "api", StringComparison.InvariantCultureIgnoreCase)) length--;
        url = string.Join("/", parts.Take(length + 1)) + "/";

        BaseAddress = url;
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// The base address.
    /// </summary>
    public string BaseAddress { get; }

    /// <summary>
    /// The <see cref="ILogger"/> used by this connection.
    /// </summary>
    public ILogger? Logger { get; private set; }

    /// <summary>
    /// Helper function for intialising the stream from a response.
    /// </summary>
    /// <remarks>
    /// This function allows for intercepting the content if needed.
    /// </remarks>
    internal Func<Connection, HttpResponseMessage, CancellationToken, Task<Stream>> InitialiseResponseStream { get; set; } =
        (_, resp, cancellationToken) => resp.Content.ReadAsStreamAsync(cancellationToken);

    /// <summary>
    /// Performs a GET operation.
    /// </summary>
    /// <param name="url">The URL to call with the GET operation.</param>
    /// <param name="throwExceptionOnFailure">Whether to throw an exception if the server returns a non-success code.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="HttpResponseMessage"/> instance containing the response from the server.</returns>
    public async Task<HttpResponseMessage> Get(
        string url,
        bool throwExceptionOnFailure = true,
        CancellationToken cancellationToken = default)
    {
        var uri = EnsureAbsoluteUri(url);
        Logger?.Debug("Sending GET to {url}", uri);
        var response = await _client
            .GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        Logger?.Debug("Received {status} from {url} - GET", response.StatusCode, url);
        if (throwExceptionOnFailure && !response.IsSuccessStatusCode) throw ConnectionException.New(url, response);
        return response;
    }

    /// <summary>
    /// Retrieves a list of entities fom the Canvas installation.
    /// </summary>
    /// <typeparam name="TItem">The type of item to retrieve.</typeparam>
    /// <param name="url">The URL to use.</param>
    /// <param name="settings">The settings to use in the list operation.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{TItem}"/> containing the entities from Canvas.</returns>
    public async IAsyncEnumerable<TItem> List<TItem>(
        string url,
        List? settings = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        settings ??= new List();
        var pageNumber = 0;
        var fullUrl = url + settings.ToParameters();
        Logger?.Debug("Listing {type} entities from {url}", typeof(TItem).Name, fullUrl);
        var cancel = false;
        while (!cancel && !string.IsNullOrEmpty(fullUrl) && pageNumber++ < settings.MaxPages)
        {
            var response = await Get(fullUrl, cancellationToken: cancellationToken);
            var stream = await InitialiseResponseStream(this, response, cancellationToken);
            await foreach (var item in JsonSerializer
                .DeserializeAsyncEnumerable<TItem>(stream, _serializerOptions, cancellationToken))
            {
                if (item == null) continue;
                yield return item;
            }

            fullUrl = GetNextLink(response.Headers);
        }
    }

    /// <summary>
    /// Performs a POST operation passing in a JSON object, and deserializing the JSON response.
    /// </summary>
    /// <param name="url">The URL to use.</param>
    /// <param name="item">The item to convert to JSON.</param>
    /// <param name="throwExceptionOnFailure">Whether to throw an exception if the server returns a non-success code.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="TItem"/> instance containing the response.</returns>
    public async Task<TItem?> PostJson<TItem>(string url, object item, bool throwExceptionOnFailure = true,
        CancellationToken cancellationToken = default) where TItem : class
    {
        var uri = EnsureAbsoluteUri(url);
        Logger?.Debug("Sending POST to {url}", uri);
        var response = await _client.PostAsJsonAsync(uri,
            item,
            _serializerOptions,
            cancellationToken);
        Logger?.Debug("Received {status} from {url} - POST", response.StatusCode, url);
        if (throwExceptionOnFailure && !response.IsSuccessStatusCode) throw ConnectionException.New(url, response);
        return await response.Content.ReadFromJsonAsync<TItem>(_serializerOptions, cancellationToken);
    }

    /// <summary>
    /// Performs a PUT operation passing in a JSON object, and deserializing the JSON response.
    /// </summary>
    /// <param name="url">The URL to use.</param>
    /// <param name="item">The item to convert to JSON.</param>
    /// <param name="throwExceptionOnFailure">Whether to throw an exception if the server returns a non-success code.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="TItem"/> instance containing the response.</returns>
    public async Task<TItem?> PutJson<TItem>(string url, object item, bool throwExceptionOnFailure = true,
        CancellationToken cancellationToken = default) where TItem : class
    {
        var uri = EnsureAbsoluteUri(url);
        Logger?.Debug("Sending PUT to {url}", uri);
        var response = await _client.PutAsJsonAsync(uri,
            item,
            _serializerOptions,
            cancellationToken);
        Logger?.Debug("Received {status} from {url} - POST", response.StatusCode, url);
        if (throwExceptionOnFailure && !response.IsSuccessStatusCode) throw ConnectionException.New(url, response);
        return await response.Content.ReadFromJsonAsync<TItem>(_serializerOptions, cancellationToken);
    }

    /// <summary>
    /// Retrieves an entity from the Canvas installation.
    /// </summary>
    /// <typeparam name="TItem">The type of item to retrieve.</typeparam>
    /// <param name="url">The URL to use.</param>
    /// <param name="parameters">The parameters to pass.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="TItem"/> instance if found; <c>null</c> otherwise.</returns>
    public async Task<TItem?> Retrieve<TItem>(
        string url,
        Parameters parameters,
        CancellationToken cancellationToken = default)
        where TItem : class
    {
        var fullUrl = url + parameters;
        Logger?.Debug("Retrieving {type} entity from {url}", typeof(TItem).Name, fullUrl);
        var response = await Get(fullUrl, false, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;
        var json = await InitialiseResponseStream(this, response, cancellationToken);
        var item = await JsonSerializer.DeserializeAsync<TItem>(json, _serializerOptions, cancellationToken);
        return item;
    }

    /// <summary>
    /// Updates the logger for the connection.
    /// </summary>
    /// <param name="logger">The <see cref="ILogger"/> to use.</param>
    public void UpdateLogger(ILogger logger)
    {
        Logger = logger.ForContext<Connection>();
    }

    /// <summary>
    /// Checks for the next link URL and returns it if there is one.
    /// </summary>
    /// <param name="headers">The response headers.</param>
    /// <returns>The next link URL if there is one, <c>null</c> otherwise.</returns>
    private static string? GetNextLink(HttpResponseHeaders headers)
    {
        if (!headers.Contains("Link")) return null;
        var header = string.Join(",", headers.NonValidated["Link"]);
        return (from link
                in header.Split(',', StringSplitOptions.RemoveEmptyEntries)
                let colonPos = link.IndexOf(';')
                let type = link[(colonPos + 1)..].Trim()
                where type == "rel=\"next\""
                select link[..colonPos].Trim() into url
                select url[1..^1]).FirstOrDefault();
    }

    /// <summary>
    /// Attempt to generate the URL for use in the requests.
    /// </summary>
    /// <param name="url">The URL to use.</param>
    /// <returns>A <see cref="Uri"/> containing the URL.</returns>
    private Uri EnsureAbsoluteUri(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) uri = new Uri(BaseAddress + url);
        return uri;
    }
}