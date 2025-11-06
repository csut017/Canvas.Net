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
    /// <returns>A <see cref="User"/> containing the current user details.</returns>
    Task<User?> Get();
}