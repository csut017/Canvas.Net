using Canvas.Core.Clients;
using Canvas.Core.Implementations;
using CommunityToolkit.Diagnostics;
using Serilog;

namespace Canvas.Core;

/// <summary>
/// Internal implementation of <see cref="IClient"/>.
/// </summary>
internal class Client
    : IClient
{
    private readonly Lazy<ICourses> _coursesClient;
    private readonly Lazy<ICurrentUser> _currentUserClient;

    /// <summary>
    /// Initialize a new <see cref="Client"/> instance.
    /// </summary>
    /// <param name="connection">The <see cref="IConnection"/> instance to use.</param>
    /// <param name="logger">An optional <see cref="ILogger"/> instance for any logging.</param>
    public Client(IConnection connection, ILogger? logger = null)
    {
        Guard.IsNotNull(connection);
        var logger1 = logger?.ForContext<Client>();

        // Initialize the underlying clients
        _coursesClient = new Lazy<ICourses>(() => new CoursesClient(connection, logger1));
        _currentUserClient = new Lazy<ICurrentUser>(() => new CurrentUserClient(connection, logger1));
    }

    /// <summary>
    /// The interface for working with courses.
    /// </summary>
    public ICourses Courseses => _coursesClient.Value;

    /// <summary>
    /// The interface for working with the current user.
    /// </summary>
    public ICurrentUser CurrentUser => _currentUserClient.Value;
}