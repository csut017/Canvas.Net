using Canvas.Core.Entities;

namespace Canvas.Core.Clients;

/// <summary>
/// Provides access to course-related functionality.
/// </summary>
public interface ICourses
{
    /// <summary>
    /// Gets the assignments interface.
    /// </summary>
    IAssignments Assignments { get; }

    /// <summary>
    /// Lists all the courses for an account.
    /// </summary>
    /// <param name="accountId">The identifier of the account.</param>
    /// <param name="settings">The settings for performing the list.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Course}"/> containing the courses for the account.</returns>
    IAsyncEnumerable<Course> ListForAccount(ulong accountId, Settings.CourseList? settings = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all the courses for an account.
    /// </summary>
    /// <param name="account">The account.</param>
    /// <param name="settings">The settings for performing the list.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Course}"/> containing the courses for the account.</returns>
    IAsyncEnumerable<Course> ListForAccount(Account account, Settings.CourseList? settings = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all the courses for the current user.
    /// </summary>
    /// <param name="settings">The settings for performing the list.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Course}"/> containing the courses for the current user.</returns>
    IAsyncEnumerable<Course> ListForCurrentUser(Settings.CourseList? settings = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the details of a course.
    /// </summary>
    /// <param name="id">The identifier of the course.</param>
    /// <param name="settings">The settings for performing the retrieve.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Course"/> instance if found; <c>null</c> otherwise.</returns>
    Task<Course?> Retrieve(ulong id, Settings.CourseItem? settings = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the details of a course.
    /// </summary>
    /// <param name="course">The <see cref="Course"/> instance to retrieve the details for.</param>
    /// <param name="settings">The settings for performing the retrieve.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Course"/> instance if found; <c>null</c> otherwise.</returns>
    Task<Course?> Retrieve(Course course, Settings.CourseItem? settings = null, CancellationToken cancellationToken = default);
}