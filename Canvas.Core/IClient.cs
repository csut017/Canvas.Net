using Canvas.Core.Clients;

namespace Canvas.Core;

/// <summary>
/// The main interface for connecting to a Canvas installation.
/// </summary>
public interface IClient
{
    /// <summary>
    /// The interface for working with courses.
    /// </summary>
    ICourse Courses { get; }

    /// <summary>
    /// The interface for working with the current user.
    /// </summary>
    ICurrentUser CurrentUser { get; }
}