using System.Collections.ObjectModel;
using Canvas.Core.Entities;

namespace Canvas.Core;

/// <summary>
/// A connection exception from Canvas.
/// </summary>
public class CanvasException
    : ConnectionException
{
    /// <summary>
    /// Initialize a new <see cref="ConnectionException"/> instance.
    /// </summary>
    /// <param name="url">The URL called.</param>
    /// <param name="message">The message.</param>
    /// <param name="errors">The <see cref="Error"/> instances from Canvas.</param>
    public CanvasException(string url, string? message, IEnumerable<Error> errors)
        : base(url, message, null)
    {
        Errors = new ReadOnlyCollection<Error>(errors.ToList());
    }

    /// <summary>
    /// Initialize a new <see cref="ConnectionException"/> instance.
    /// </summary>
    /// <param name="url">The URL called.</param>
    /// <param name="message">The message.</param>
    /// <param name="errors">The <see cref="Error"/> instances from Canvas.</param>
    /// <param name="innerException">An inner <see cref="Exception"/> instance.</param>
    public CanvasException(string url, string? message, IEnumerable<Error> errors, Exception? innerException)
        : base(url, message, innerException)
    {
        Errors = new ReadOnlyCollection<Error>(errors.ToList());
    }

    /// <summary>
    /// The errors from Canvas.
    /// </summary>
    public IReadOnlyCollection<Error> Errors { get; }
}