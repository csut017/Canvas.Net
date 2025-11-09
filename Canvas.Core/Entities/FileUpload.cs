namespace Canvas.Core.Entities;

/// <summary>
/// A file to upload to Canvas.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public record FileUpload
    : IDisposable
{
    /// <summary>
    /// The name of the file.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The size of the file.
    /// </summary>
    public long? Size { get; init; }

    /// <summary>
    /// The underlying <see cref="Stream"/> instance.
    /// </summary>
    public required Stream Stream { get; init; }

    /// <summary>
    /// The file type.
    /// </summary>
    public string? Type { get; init; }

    private string DebuggerDisplay => Name + (Size.HasValue ? $" [{Size} bytes]" : string.Empty);

    /// <summary>
    /// Generates a new <see cref="FileUpload"/> instance from a file path.
    /// </summary>
    /// <param name="info">A <see cref="FileInfo"/> containg the details of the file.</param>
    /// <param name="type">The type of file.</param>
    /// <returns>The new <see cref="FileUpload"/> instance.</returns>
    public static FileUpload New(FileInfo info, string? type = null)
    {
        if (!info.Exists) throw new FileNotFoundException($"File {info.Name} does not exist", info.FullName);
        var value = new FileUpload
        {
            Name = info.Name,
            Stream = info.Open(FileMode.Open),
            Size = info.Length,
            Type = type,
        };
        return value;
    }

    /// <summary>
    /// Generates a new <see cref="FileUpload"/> instance from a file path.
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <returns>The new <see cref="FileUpload"/> instance.</returns>
    public static FileUpload New(string path, string? type = null)
    {
        var info = new FileInfo(path);
        return New(info, type);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Stream.Dispose();
    }

    /// <summary>
    /// Generates the arguments for uploading the file.
    /// </summary>
    /// <returns>A <see cref="Dictionary{String, String}"/> containing the arguments for the upload.</returns>
    public Dictionary<string, string> GenerateUploadArgs()
    {
        var args = new Dictionary<string, string>
        {
            ["name"] = Name,
        };
        if (Size.HasValue) args.Add("size", Size.Value.ToString());
        if (!string.IsNullOrEmpty(Type)) args.Add("content_type", Type);
        return args;
    }
}