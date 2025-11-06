using Canvas.Core.Settings;

namespace Canvas.Core;

/// <summary>
/// A connection to a Canvas installation.
/// </summary>
public interface IConnection
{
    /// <summary>
    /// Performs a HTTP GET operation.
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
    /// <param name="parameters">The parameters to pass.</param>
    /// <param name="settings">The settings to use in the list operation.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{TItem}"/> containing the entities from Canvas.</returns>
    IAsyncEnumerable<TItem> List<TItem>(
        string url,
        Parameters parameters,
        List? settings = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a POST operation and passes in a set of form values.
    /// </summary>
    /// <param name="url">The URL to use.</param>
    /// <param name="formValues">The values in the form.</param>
    /// <param name="throwExceptionOnFailure">Whether to throw an exception if the server returns a non-success code.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <remarks>
    /// This operation is a low-level operation on the underlying connection. It should only be used when there
    /// is a need to directly manipulate the HTTP data.
    /// </remarks>
    /// <returns>An <see cref="HttpResponseMessage"/> instance containing the response from the server.</returns>
    Task<HttpResponseMessage> Post(
        string url,
        IDictionary<string, string> formValues,
        bool throwExceptionOnFailure = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a POST operation using a <see cref="Stream"/>.
    /// </summary>
    /// <param name="url">The URL to use.</param>
    /// <param name="stream">The <see cref="Stream"/> instance to use.</param>
    /// <param name="streamName">The name of the stream</param>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="formValues">Any additional parameters to send.</param>
    /// <param name="throwExceptionOnFailure">Whether to throw an exception if the server returns a non-success code.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <remarks>
    /// This operation is a low-level operation on the underlying connection. It should only be used when there
    /// is a need to directly manipulate the HTTP data.
    /// </remarks>
    /// <returns>An <see cref="HttpResponseMessage"/> instance containing the response from the server.</returns>
    Task<HttpResponseMessage> Post(
        string url,
        Stream stream,
        string streamName,
        string fileName,
        IDictionary<string, string>? formValues = null,
        bool throwExceptionOnFailure = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a POST operation passing in a set of form values, and deserializing the JSON response.
    /// </summary>
    /// <param name="url">The URL to use.</param>
    /// <param name="formValues">The values in the form.</param>
    /// <param name="throwExceptionOnFailure">Whether to throw an exception if the server returns a non-success code.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="TItem"/> instance containing the response.</returns>
    Task<TItem?> Post<TItem>(
        string url,
        IDictionary<string, string> formValues,
        bool throwExceptionOnFailure = true,
        CancellationToken cancellationToken = default)
        where TItem : class;

    /// <summary>
    /// Performs a PUT operation and passes in a set of form values.
    /// </summary>
    /// <param name="url">The URL to use.</param>
    /// <param name="formValues">The values in the form.</param>
    /// <param name="throwExceptionOnFailure">Whether to throw an exception if the server returns a non-success code.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="HttpResponseMessage"/> instance containing the response from the server.</returns>
    /// <remarks>
    /// This operation is a low-level operation on the underlying connection. It should only be used when there
    /// is a need to directly manipulate the HTTP data.
    /// </remarks>
    Task<HttpResponseMessage> Put(
        string url,
        IDictionary<string, string> formValues,
        bool throwExceptionOnFailure = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a PUT operation passing in a set of form values, and deserializing the JSON response.
    /// </summary>
    /// <param name="url">The URL to use.</param>
    /// <param name="formValues">The values in the form.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="TItem"/> instance containing the response.</returns>
    Task<TItem?> Put<TItem>(
        string url,
        IDictionary<string, string> formValues,
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
    Task<TItem> UploadFile<TItem>(
        Stream stream,
        string url,
        Dictionary<string, string> formValues,
        string fileName,
        CancellationToken cancellationToken = default)
        where TItem : class;
}