using Canvas.Core.Settings;

namespace Canvas.Core;

/// <summary>
/// A connection to a Canvas installation.
/// </summary>
public interface IConnection
{
    /// <summary>
    /// Performs an HTTP GET operation.
    /// </summary>
    /// <param name="url">The URL to call with the GET operation.</param>
    /// <param name="throwExceptionOnFailure">Whether to throw an exception if the server returns a non-success code.</param>
    /// <returns>An <see cref="HttpResponseMessage"/> instance containing the response from the server.</returns>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <remarks>
    /// This operation is a low-level operation on the underlying connection. It should only be used when there
    /// is a need to directly manipulate the HTTP data.
    /// </remarks>
    Task<HttpResponseMessage> Get(
        string url,
        bool throwExceptionOnFailure = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of entities fom the Canvas installation.
    /// </summary>
    /// <typeparam name="TItem">The type of item to retrieve.</typeparam>
    /// <param name="url">The URL to use.</param>
    /// <param name="settings">The settings to use in the list operation.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{TItem}"/> containing the entities from Canvas.</returns>
    IAsyncEnumerable<TItem> List<TItem>(
        string url,
        List? settings = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a POST operation passing in a JSON object, and deserializing the JSON response.
    /// </summary>
    /// <param name="url">The URL to use.</param>
    /// <param name="item">The item to convert to JSON.</param>
    /// <param name="throwExceptionOnFailure">Whether to throw an exception if the server returns a non-success code.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="TItem"/> instance containing the response.</returns>
    Task<TItem?> PostJson<TItem>(
        string url,
        object item,
        bool throwExceptionOnFailure = true,
        CancellationToken cancellationToken = default)
        where TItem : class;

    /// <summary>
    /// Performs a PUT operation passing in a JSON object, and deserializing the JSON response.
    /// </summary>
    /// <param name="url">The URL to use.</param>
    /// <param name="item">The item to convert to JSON.</param>
    /// <param name="throwExceptionOnFailure">Whether to throw an exception if the server returns a non-success code.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="TItem"/> instance containing the response.</returns>
    Task<TItem?> PutJson<TItem>(
        string url,
        object item,
        bool throwExceptionOnFailure = true,
        CancellationToken cancellationToken = default)
        where TItem : class;

    /// <summary>
    /// Retrieves an entity from the Canvas installation.
    /// </summary>
    /// <typeparam name="TItem">The type of item to retrieve.</typeparam>
    /// <param name="url">The URL to use.</param>
    /// <param name="parameters">The parameters to pass.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="TItem"/> instance if found; <c>null</c> otherwise.</returns>
    Task<TItem?> Retrieve<TItem>(
        string url,
        Parameters parameters,
        CancellationToken cancellationToken = default)
        where TItem : class;
}