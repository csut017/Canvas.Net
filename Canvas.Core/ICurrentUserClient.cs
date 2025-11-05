using Canvas.Core.Entities;

namespace Canvas.Core;

/// <summary>
/// Provides operations for the current user.
/// </summary>
public interface ICurrentUserClient
{
    /// <summary>
    /// Retrieves the details of the current user.
    /// </summary>
    /// <returns>A <see cref="User"/> containing the current user details.</returns>
    Task<User?> Get();
}