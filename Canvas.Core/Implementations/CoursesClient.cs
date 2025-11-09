using Canvas.Core.Clients;
using Canvas.Core.Entities;
using Canvas.Core.Settings;
using CommunityToolkit.Diagnostics;
using Serilog;

namespace Canvas.Core.Implementations;

/// <summary>
/// Default implementation of <see cref="ICourses"/>.
/// </summary>
internal class CoursesClient
    : ICourses
{
    private readonly Lazy<IAssignments> _assignmentsClient;
    private readonly IConnection _connection;
    private readonly ILogger? _logger;

    /// <summary>
    /// Initialises a new <see cref="ICurrentUser"/> instance.
    /// </summary>
    /// <param name="connection">The underlying connection.</param>
    /// <param name="logger">An optional logger.</param>
    public CoursesClient(IConnection connection, ILogger? logger = null)
    {
        Guard.IsNotNull(connection);
        _connection = connection;
        _logger = logger?.ForContext<CoursesClient>();

        // Initialize the underlying clients
        _assignmentsClient = new Lazy<IAssignments>(() => new AssignmentsClient(connection, _logger));
    }

    /// <summary>
    /// Gets the Assignments interface.
    /// </summary>
    public IAssignments Assignments => _assignmentsClient.Value;

    /// <summary>
    /// Lists all the courses for an account.
    /// </summary>
    /// <param name="accountId">The identifier of the account.</param>
    /// <param name="settings">The settings for performing the list.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Course}"/> containing the courses for the account.</returns>
    public IAsyncEnumerable<Course> ListForAccount(ulong accountId, CourseList? settings = null,
        CancellationToken cancellationToken = default)
    {
        settings ??= new();
        settings = settings with { Options = settings.Options | CourseInclude.Term };

        _logger?.Debug("Listing courses for account with id {id}", accountId);
        return _connection.List<Course>(
            $"/api/v1/accounts/{accountId}/courses",
            settings,
            cancellationToken);
    }

    /// <summary>
    /// Lists all the courses for an account.
    /// </summary>
    /// <param name="account">The account.</param>
    /// <param name="settings">The settings for performing the list.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Course}"/> containing the courses for the account.</returns>
    public IAsyncEnumerable<Course> ListForAccount(Account account, CourseList? settings = null,
        CancellationToken cancellationToken = default)
    {
        return ListForAccount(account.Id, settings, cancellationToken);
    }

    /// <summary>
    /// Lists all the courses for the current user.
    /// </summary>
    /// <param name="settings">The settings for performing the list.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Course}"/> containing the courses for the current user.</returns>
    public IAsyncEnumerable<Course> ListForCurrentUser(CourseList? settings = null, CancellationToken cancellationToken = default)
    {
        settings ??= new();
        settings = settings with { Options = settings.Options | CourseInclude.Term };

        _logger?.Debug("Listing courses for current user");
        return _connection.List<Course>(
            "/api/v1/courses",
            settings,
            cancellationToken);
    }

    /// <summary>
    /// Retrieves the details of a course.
    /// </summary>
    /// <param name="id">The identifier of the course.</param>
    /// <param name="settings">The settings for performing the retrieve.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Course"/> instance if found; <c>null</c> otherwise.</returns>
    public Task<Course?> Retrieve(ulong id, CourseItem? settings = null, CancellationToken cancellationToken = default)
    {
        settings ??= new CourseItem();
        settings = settings with { Options = settings.Options | CourseInclude.Term };

        _logger?.Debug("Retrieving course with id {courseId}", id);
        return _connection.Retrieve<Course>(
            $"/api/v1/courses/{id}",
            settings.ToParameters(),
            cancellationToken);
    }

    /// <summary>
    /// Retrieves the details of a course.
    /// </summary>
    /// <param name="course">The <see cref="Course"/> instance to retrieve the details for.</param>
    /// <param name="settings">The settings for performing the retrieve.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Course"/> instance if found; <c>null</c> otherwise.</returns>
    public Task<Course?> Retrieve(Course course, CourseItem? settings = null, CancellationToken cancellationToken = default)
    {
        return Retrieve(course.Id, settings, cancellationToken);
    }
}