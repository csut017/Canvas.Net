using Canvas.Core.Settings;
using CommunityToolkit.Diagnostics;
using Serilog;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Canvas.Core.Http;

/// <summary>
/// A <see cref="IConnection"/> instance that connects to a Canvas server using HTTP.
/// </summary>
public class Connection
    : IConnection
{
    private readonly HttpClient _client;
    private readonly ILogger? _logger;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>
    /// Initialize a new <see cref="Connection"/> instance using a URL and token.
    /// </summary>
    /// <param name="url">The URL to the Canvas server.</param>
    /// <param name="token">The default user token to use.</param>
    /// <param name="logger">An optional <see cref="ILogger"/> to use for any logging.</param>
    /// <param name="client">An optional <see cref="HttpClient"/> instance.</param>
    public Connection(
        string url,
        string token,
        ILogger? logger = null,
        HttpClient? client = null)
    {
        Guard.IsNotNullOrWhiteSpace(url);
        Guard.IsNotNullOrWhiteSpace(token);

        _logger = logger?.ForContext<Connection>();

        _client = client ?? new HttpClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Do some basic cleaning operations - it won't change everything, but it will catch the obvious ones
        var parts = url.Split('/');
        var length = parts.Length - 1;
        if (string.IsNullOrEmpty(parts[length])) length--;
        if (string.Equals(parts[length], "v1", StringComparison.InvariantCultureIgnoreCase)) length--;
        if (string.Equals(parts[length], "api", StringComparison.InvariantCultureIgnoreCase)) length--;
        url = string.Join("/", parts.Take(length + 1)) + "/";

        logger?.Debug("Setting connection base address to {url}", url);
        BaseAddress = url;
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// The base address.
    /// </summary>
    public string BaseAddress { get; }

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
        _logger?.Debug("Sending GET to {url}", uri);
        var response = await _client
            .GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        _logger?.Debug("Received {status} from {url} - GET", response.StatusCode, url);
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
        _logger?.Debug("Listing {type} entities from {url}", typeof(TItem).Name, fullUrl);
        var cancel = false;
        while (!cancel && !string.IsNullOrEmpty(fullUrl) && pageNumber++ < settings.MaxPages)
        {
            var response = await Get(fullUrl, cancellationToken: cancellationToken);
            var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
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
    /// Performs a POST operation and passes in a set of form values.
    /// </summary>
    /// <param name="url">The URL to use.</param>
    /// <param name="formValues">The values in the form.</param>
    /// <param name="throwExceptionOnFailure">Whether to throw an exception if the server returns a non-success code.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="HttpResponseMessage"/> instance containing the response from the server.</returns>
    public async Task<HttpResponseMessage> Post(
        string url,
        IDictionary<string, string> formValues,
        bool throwExceptionOnFailure = true,
        CancellationToken cancellationToken = default)
    {
        var uri = EnsureAbsoluteUri(url);
        _logger?.Debug("Sending POST to {url}", uri);
        var response = await _client.PostAsync(uri,
            new FormUrlEncodedContent(formValues), cancellationToken);
        _logger?.Debug("Received {status} from {url} - POST", response.StatusCode, url);
        if (throwExceptionOnFailure && !response.IsSuccessStatusCode) throw ConnectionException.New(url, response);
        return response;
    }

    /// <summary>
    /// Performs a POST operation and passes a stream.
    /// </summary>
    /// <param name="url">The URL to use.</param>
    /// <param name="stream">The stream to use.</param>
    /// <param name="streamName">The name of the stream</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="formValues">Any additional parameters to send.</param>
    /// <param name="throwExceptionOnFailure">Whether to throw an exception if the server returns a non-success code.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="HttpResponseMessage"/> instance containing the response from the server.</returns>
    public async Task<HttpResponseMessage> Post(
        string url,
        Stream stream,
        string streamName,
        string fileName,
        IDictionary<string, string>? formValues = null,
        bool throwExceptionOnFailure = true,
        CancellationToken cancellationToken = default)
    {
        using var streamContent = new StreamContent(stream);
        using var content = new MultipartFormDataContent();
        if (formValues != null)
        {
            foreach (var value in formValues)
            {
                content.Add(new StringContent(value.Value), value.Key);
            }
        }

        content.Add(streamContent, streamName, Path.GetFileName(fileName));
        var uri = EnsureAbsoluteUri(url);
        _logger?.Debug("Sending GET to {url}", uri);
        var response = await _client.PostAsync(uri, content, cancellationToken);
        _logger?.Debug("Received {status} from {url} - POST", response.StatusCode, url);
        if (throwExceptionOnFailure && !response.IsSuccessStatusCode) throw ConnectionException.New(url, response);
        return response;
    }

    /// <summary>
    /// Performs a POST operation passing in a set of form values, and deserializing the JSON response.
    /// </summary>
    /// <param name="url">The URL to use.</param>
    /// <param name="formValues">The values in the form.</param>
    /// <param name="throwExceptionOnFailure">Whether to throw an exception if the server returns a non-success code.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="TItem"/> instance containing the response.</returns>
    public async Task<TItem?> Post<TItem>(
        string url,
        IDictionary<string, string> formValues,
        bool throwExceptionOnFailure = true,
        CancellationToken cancellationToken = default) where TItem : class
    {
        var response = await Post(url, formValues, throwExceptionOnFailure, cancellationToken);
        var json = await response.Content.ReadAsStreamAsync(cancellationToken);
        var item = await JsonSerializer.DeserializeAsync<TItem>(json, _serializerOptions, cancellationToken);
        return item;
    }

    /// <summary>
    /// Performs a PUT operation and passes in a set of form values.
    /// </summary>
    /// <param name="url">The URL to use.</param>
    /// <param name="formValues">The values in the form.</param>
    /// <param name="throwExceptionOnFailure">Whether to throw an exception if the server returns a non-success code.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="HttpResponseMessage"/> instance containing the response from the server.</returns>
    public async Task<HttpResponseMessage> Put(
        string url,
        IDictionary<string, string> formValues,
        bool throwExceptionOnFailure = true,
        CancellationToken cancellationToken = default)
    {
        var uri = EnsureAbsoluteUri(url);
        _logger?.Debug("Sending PUT to {url}", uri);
        var response = await _client.PutAsync(
            uri,
            new FormUrlEncodedContent(formValues),
            cancellationToken);
        _logger?.Debug("Received {status} from {url} - PUT", response.StatusCode, url);
        if (throwExceptionOnFailure && !response.IsSuccessStatusCode) throw ConnectionException.New(url, response);
        return response;
    }

    /// <summary>
    /// Performs a PUT operation passing in a set of form values, and deserializing the JSON response.
    /// </summary>
    /// <param name="url">The URL to use.</param>
    /// <param name="formValues">The values in the form.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="TItem"/> instance containing the response.</returns>
    public async Task<TItem?> Put<TItem>(
        string url,
        IDictionary<string, string> formValues,
        CancellationToken cancellationToken = default) where TItem : class
    {
        var response = await Put(url, formValues, cancellationToken: cancellationToken);
        var json = await response.Content.ReadAsStreamAsync(cancellationToken);
        var item = await JsonSerializer.DeserializeAsync<TItem>(json, _serializerOptions, cancellationToken);
        return item;
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
        _logger?.Debug("Retrieving {type} entity from {url}", typeof(TItem).Name, fullUrl);
        var response = await Get(fullUrl, false, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStreamAsync(cancellationToken);
        var item = await JsonSerializer.DeserializeAsync<TItem>(json, _serializerOptions, cancellationToken);
        return item;
    }

    /// <summary>
    /// Uploads a file to Canvas.
    /// </summary>
    /// <typeparam name="TItem">The type of entity to receive at the end of the process.</typeparam>
    /// <param name="stream">The <see cref="Stream"/> to upload.</param>
    /// <param name="url">The URL to trigger the process.</param>
    /// <param name="formValues">The additional values to pass.</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="TItem"/> instance containing the final response.</returns>
    public async Task<TItem> UploadFile<TItem>(
        Stream stream,
        string url,
        Dictionary<string, string> formValues,
        string fileName,
        CancellationToken cancellationToken = default) where TItem : class
    {
        // Uploading files is a three-stage process: see https://canvas.instructure.com/doc/api/file.file_uploads.html
        // Step 1: start upload process
        var response = await Post(url, formValues, cancellationToken: cancellationToken);
        var uploadJson = await response.Content.ReadAsStreamAsync(cancellationToken);
        var uploadToken = await JsonSerializer.DeserializeAsync<FileUploadToken>(uploadJson, _serializerOptions, cancellationToken);
        if (string.IsNullOrEmpty(uploadToken?.Url)) throw new ConnectionException(url, "Upload failed: invalid JSON token received");

        // Step 2: upload the file data
        _logger?.Debug("Sending data to {url}", uploadToken.Url);
        response = await Post(uploadToken.Url, stream, fileName, fileName, uploadToken.Parameters, false, cancellationToken);
        if ((int)response.StatusCode < 200 || (int)response.StatusCode > 399) throw ConnectionException.New(uploadToken.Url, response);

        // Step 3: retrieve file details
        TItem? newFile;
        if (response.StatusCode == HttpStatusCode.OK)
        {
            // Details were directly returned
            _logger?.Debug("Received {status} - parsing response", response.StatusCode);
            var json = await response.Content.ReadAsStreamAsync(cancellationToken);
            newFile = await JsonSerializer.DeserializeAsync<TItem>(json, _serializerOptions, cancellationToken);
            return newFile
                ?? throw new ConnectionException(uploadToken.Url, "Upload failed: invalid final JSON received");
        }

        // Need to follow the redirect first
        var location = response.Headers.Location
            ?? throw new ConnectionException(uploadToken.Url, "Upload failed: no redirect location received");
        _logger?.Debug("Received {status} - getting {url}", response.StatusCode, location);
        newFile = await Retrieve<TItem>(location.ToString(), [], cancellationToken);
        return newFile
            ?? throw new ConnectionException(location.ToString(), "Upload failed: invalid final JSON received");
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