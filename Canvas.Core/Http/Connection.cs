using Canvas.Core.Entities;
using Canvas.Core.Settings;
using CommunityToolkit.Diagnostics;
using Serilog;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Canvas.Core.Http;

/// <summary>
/// A <see cref="IConnection"/> instance that connects to a Canvas server using HTTP.
/// </summary>
public class Connection
    : ILoggingConnection
{
    private const string JsonMediaType = "application/json";
    private readonly HttpClient _client;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
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
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMediaType));
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
    /// Helper function for initialising the stream from a response.
    /// </summary>
    /// <remarks>
    /// This function allows for intercepting the content if needed.
    /// </remarks>
    internal Func<Connection, HttpResponseMessage, CancellationToken, Task<Stream>> InitialiseResponseStream { get; set; } =
        (_, resp, cancellationToken) => resp.Content.ReadAsStreamAsync(cancellationToken);

    /// <summary>
    /// Helper function for processing the stream for a request.
    /// </summary>
    /// <remarks>
    /// This function allows for intercepting the output if needed.
    /// </remarks>
    internal Func<Connection, MemoryStream, CancellationToken, Task> ProcessOutputStream { get; set; } = (_, _, _) => Task.CompletedTask;

    /// <summary>
    /// Checks the response and generates an exception if it has failed.
    /// </summary>
    /// <param name="url">The URL called.</param>
    /// <param name="response">The <see cref="HttpResponseMessage"/> instance.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A new <see cref="ConnectionException"/> instance.</returns>
    public async Task CheckResponse(string url, HttpResponseMessage? response, CancellationToken cancellationToken)
    {
        if (response == null) throw new ConnectionException(url, "Something went wrong while calling Canvas");
        if (response.IsSuccessStatusCode) return;

        using var stream = new MemoryStream();
        await response.Content.CopyToAsync(stream, cancellationToken);

        // Canvas uses two different forms of error responses - so we need to try both of them
        stream.Seek(0, SeekOrigin.Begin);
        Exception? error = null;
        try
        {
            var errors = await JsonSerializer.DeserializeAsync<ErrorResponseList>(stream, _serializerOptions, cancellationToken);
            Guard.IsNotNull(errors);
            error = new CanvasException(
                url,
                $"Canvas returned a non-success response code [{response.StatusCode}]",
                errors.Errors);
        }
        catch
        {
            // Ignore the error - we will read an output the response in the next step
        }
        if (error != null) throw error;

        stream.Seek(0, SeekOrigin.Begin);
        try
        {
            var errors = await JsonSerializer.DeserializeAsync<ErrorResponseDictionary>(stream, _serializerOptions, cancellationToken);
            Guard.IsNotNull(errors);
            error = new CanvasException(
                url,
                $"Canvas returned a non-success response code [{response.StatusCode}]",
                errors.Errors.SelectMany(kvp => kvp.Value));
        }
        catch
        {
            // Ignore the error - we will read an output the response in the next step
        }
        if (error != null) throw error;

        // If all else fails, just extract the content as a string and return that
        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(stream);
        throw new ConnectionException(url, $"Canvas returned a non-success response code [{response.StatusCode}]")
        {
            Content = await reader.ReadToEndAsync(cancellationToken)
        };
    }

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
        if (throwExceptionOnFailure) await CheckResponse(url, response, cancellationToken);
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
        using var content = await GenerateJsonContent(item, cancellationToken);
        var response = await _client.PostAsync(
            uri,
            content,
            cancellationToken);
        Logger?.Debug("Received {status} from {url} - POST", response.StatusCode, url);
        if (throwExceptionOnFailure) await CheckResponse(url, response, cancellationToken);

        var json = await InitialiseResponseStream(this, response, cancellationToken);
        var responseItem = await JsonSerializer.DeserializeAsync<TItem>(json, _serializerOptions, cancellationToken);
        return responseItem;
    }

    /// <summary>
    /// Performs a PUT operation passing in a JSON object, and deserializing the JSON response.
    /// </summary>
    /// <param name="url">The URL to use.</param>
    /// <param name="values">The values to upload to Canvas.</param>
    /// <param name="throwExceptionOnFailure">Whether to throw an exception if the server returns a non-success code.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="TItem"/> instance containing the response.</returns>
    public async Task<TItem?> PutForm<TItem>(string url, Parameters values, bool throwExceptionOnFailure = true,
        CancellationToken cancellationToken = default) where TItem : class
    {
        var uri = EnsureAbsoluteUri(url);
        Logger?.Debug("Sending PUT to {url}", uri);
        var content = new FormUrlEncodedContent(
            values.Select(p => KeyValuePair.Create(p.Name, p.Value)));
        var response = await _client.PutAsync(
            uri,
            content,
            cancellationToken);
        Logger?.Debug("Received {status} from {url} - POST", response.StatusCode, url);
        if (throwExceptionOnFailure) await CheckResponse(url, response, cancellationToken);

        var json = await InitialiseResponseStream(this, response, cancellationToken);
        var responseItem = await JsonSerializer.DeserializeAsync<TItem>(json, _serializerOptions, cancellationToken);
        return responseItem;
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
        using var content = await GenerateJsonContent(item, cancellationToken);
        var response = await _client.PutAsync(
            uri,
            content,
            cancellationToken);
        Logger?.Debug("Received {status} from {url} - POST", response.StatusCode, url);
        if (throwExceptionOnFailure) await CheckResponse(url, response, cancellationToken);

        var json = await InitialiseResponseStream(this, response, cancellationToken);
        var responseItem = await JsonSerializer.DeserializeAsync<TItem>(json, _serializerOptions, cancellationToken);
        return responseItem;
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
    /// Uploads a file to Canvas.
    /// </summary>
    /// <typeparam name="TItem">The type of entity to receive at the end of the process.</typeparam>
    /// <param name="url">The URL to trigger the process.</param>
    /// <param name="values">The additional values to pass.</param>
    /// <param name="file">The file to upload.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="TItem"/> instance containing the final response.</returns>
    public Task<TItem> UploadFile<TItem>(
        string url,
        Parameters values,
        FileUpload file,
        CancellationToken cancellationToken = default) where TItem : class
    {
        throw new NotImplementedException();
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
        if (url[0] == '/') url = url[1..];
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) uri = new Uri(BaseAddress + url);
        return uri;
    }

    private async Task<StreamContent> GenerateJsonContent(object item, CancellationToken cancellationToken)
    {
        var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, item, _serializerOptions, cancellationToken);
        stream.Seek(0, SeekOrigin.Begin);
        await ProcessOutputStream(this, stream, cancellationToken);
        var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue(JsonMediaType);
        return content;
    }

    private record ErrorResponseList
    {
        public required Error[] Errors { get; init; }
    }

    private record ErrorResponseDictionary
    {
        public required Dictionary<string, Error[]> Errors { get; init; }
    }
}