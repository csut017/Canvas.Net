using Canvas.Core.Entities;

namespace Canvas.Core.Clients;

/// <summary>
/// Provides operations for the current user.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Retrieves the details of the current user.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="User"/> containing the current user details.</returns>
    Task<User?> Get(CancellationToken cancellationToken = default);
}