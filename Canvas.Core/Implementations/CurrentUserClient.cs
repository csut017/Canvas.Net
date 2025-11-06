using Canvas.Core.Clients;
using Canvas.Core.Entities;
using CommunityToolkit.Diagnostics;
using Serilog;

namespace Canvas.Core.Implementations;

/// <summary>
/// Default implementation of <see cref="ICurrentUser"/>.
/// </summary>
internal class CurrentUserClient
    : ICurrentUser
{
    private readonly IConnection _connection;
    private readonly ILogger? _logger;

    /// <summary>
    /// Initialises a new <see cref="ICurrentUser"/> instance.
    /// </summary>
    /// <param name="connection">The underlying connection.</param>
    /// <param name="logger">An optional logger.</param>
    public CurrentUserClient(IConnection connection, ILogger? logger = null)
    {
        Guard.IsNotNull(connection);
        _connection = connection;
        _logger = logger?.ForContext<CurrentUserClient>();
    }

    /// <summary>
    /// Retrieves the details of the current user.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="User"/> containing the current user details.</returns>
    public Task<User?> Get(CancellationToken cancellationToken = default)
    {
        _logger?.Debug("Retrieving current user details");
        return _connection.Retrieve<User>(
            "/api/v1/users/self",
            [],
            cancellationToken);
    }
}