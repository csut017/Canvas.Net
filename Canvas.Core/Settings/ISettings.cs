namespace Canvas.Core.Settings;

/// <summary>
/// Settings that can be converted into parameters.
/// </summary>
public interface ISettings
{
    /// <summary>
    /// Generates a set of parameters with the current values from these options.
    /// </summary>
    /// <returns>The new <see cref="Parameters"/> instance containing the options.</returns>
    Parameters ToParameters();
}