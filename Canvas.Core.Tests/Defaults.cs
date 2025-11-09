using System.Text.Json;

namespace Canvas.Core.Tests;

public static class Defaults
{
    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}