namespace Canvas.Core.Entities;

public record Error
{
    public string? Attribute { get; init; }
    public string? Type { get; init; }
    public string? Message { get; init; }
}