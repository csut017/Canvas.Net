namespace Canvas.Core.Entities;

/// <summary>
/// A course offering in Canvas.
/// </summary>
[DebuggerDisplay($"{{{nameof(Code)}}} [{{{nameof(Id)}}}]")]
public record Course
    : EntityWithIdAndName
{
    /// <summary>
    /// The short code of the course.
    /// </summary>
    [JsonPropertyName("course_code")]
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// The default view to display when a user first views the course in a session.
    /// </summary>
    [JsonPropertyName("default_view")]
    public string? DefaultView { get; init; }

    /// <summary>
    /// A flag indicating whether the course is available or not.
    /// </summary>
    public bool IsAvailable => string.Equals(State, "available", StringComparison.InvariantCultureIgnoreCase);

    /// <summary>
    /// The total number of students in the course.
    /// </summary>
    [JsonPropertyName("total_students")]
    public int NumberOfStudents { get; init; }

    /// <summary>
    /// The current state of the course.
    /// </summary>
    [JsonPropertyName("workflow_state")]
    public string State { get; init; } = "Unknown";

    /// <summary>
    /// The allocated teachers in the course.
    /// </summary>
    public List<User>? Teachers { get; init; }

    /// <summary>
    /// The <see cref="Term"/> that the course is offered in.
    /// </summary>
    public Term? Term { get; init; }
}