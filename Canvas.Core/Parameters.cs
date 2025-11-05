using System.Net;
using System.Reflection;
using Humanizer;

namespace Canvas.Core;

/// <summary>
/// The parameters for an API call.
/// </summary>
public class Parameters
    : List<Parameter>
{
    /// <summary>
    /// Adds a new string parameter.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    public void Add(string name, string value)
    {
        Add(new Parameter
        {
            Name = name,
            Value = value
        });
    }

    /// <summary>
    /// Adds a new integer parameter.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    public void Add(string name, int value)
    {
        Add(new Parameter
        {
            Name = name,
            Value = value.ToString()
        });
    }

    /// <summary>
    /// Adds a new boolean parameter.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    public void Add(string name, bool value)
    {
        Add(new Parameter
        {
            Name = name,
            Value = value ? "true" : "false",
        });
    }

    /// <summary>
    /// Adds a new unsigned long parameter.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    public void Add(string name, ulong value)
    {
        Add(new Parameter
        {
            Name = name,
            Value = value.ToString()
        });
    }

    /// <summary>
    /// Adds a new enumeration parameter.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    public void Add<TEnum>(string name, TEnum value)
        where TEnum : struct, Enum
    {
        var isFlags = typeof(TEnum).GetCustomAttribute<FlagsAttribute>() != null;
        if (!isFlags)
        {
            Add(new Parameter
            {
                Name = name,
                Value = value.ToString().ToLowerInvariant(),
            });
            return;
        }

        foreach (var flag in Enum.GetValues<TEnum>())
        {
            if (flag.Equals(default(TEnum)) || !value.HasFlag(flag)) continue;

            Add(new Parameter
            {
                Name = name,
                Value = flag.ToString()
                    .Underscore()
                    .ToLowerInvariant(),
            });
        }
    }

    /// <summary>
    /// Converts the parameters into an argument string for an HTTP request.
    /// </summary>
    /// <returns>A <see cref="String"/> containing the arguments for an HTTP request</returns>
    public override string ToString()
    {
        if (Count == 0) return string.Empty;
        var output = "?" + string.Join("&", this.Select(p => $"{p.Name}={WebUtility.UrlEncode(p.Value)}"));
        return output;
    }
}