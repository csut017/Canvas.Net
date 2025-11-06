using Canvas.Core.Clients;

namespace Canvas.Core;

/// <summary>
/// Internal implementation of <see cref="IClient"/>.
/// </summary>
internal class Client
    : IClient
{
    /// <summary>
    /// The interface for working with courses.
    /// </summary>
    public ICourse Courses => throw new NotImplementedException();

    /// <summary>
    /// The interface for working with the current user.
    /// </summary>
    public ICurrentUser CurrentUser => throw new NotImplementedException();
}