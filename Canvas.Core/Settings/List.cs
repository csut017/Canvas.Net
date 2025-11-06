namespace Canvas.Core.Settings;

/// <summary>
/// The settings to use when listing data from the server.
/// </summary>
public record List
    : ISettings
{
    /// <summary>
    /// The maximum possible page size.
    /// </summary>
    public const int MaxPageSize = 100;

    /// <summary>
    /// The maximum number of pages.
    /// </summary>
    public int MaxPages { get; init; } = int.MaxValue;

    /// <summary>
    /// The number of items in each page.
    /// </summary>
    public int? PageSize { get; set; } = 50;
    /// <summary>
    /// The starting page.
    /// </summary>
    public int? PageStart { get; set; }

    /// <summary>
    /// Generates a set of parameters with the current values from these options.
    /// </summary>
    /// <returns>The new <see cref="Parameters"/> instance containing the options.</returns>
    public Parameters ToParameters()
    {
        var parameters = new Parameters();
        AppendParameters(parameters);
        return parameters;
    }

    /// <summary>
    /// Appends the parameters to a <see cref="Parameters"/> instance.
    /// </summary>
    /// <param name="parameters">The <see cref="Parameters"/> instance.</param>
    protected virtual void AppendParameters(Parameters parameters)
    {
        var pageSize = PageSize.GetValueOrDefault(50);
        parameters.Add("per_page", pageSize > MaxPageSize ? MaxPageSize : pageSize);
        if (PageStart.HasValue) parameters.Add("page", PageStart.Value);
    }
}