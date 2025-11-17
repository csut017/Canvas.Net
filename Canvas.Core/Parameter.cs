namespace Canvas.Core;

/// <summary>
/// A parameter for an API call.
/// </summary>
[DebuggerDisplay($"{{{nameof(Name)}}}={{{nameof(Value)}}}")]
public record Parameter(string Name, string Value);