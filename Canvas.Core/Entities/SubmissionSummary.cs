namespace Canvas.Core.Entities;

/// <summary>
/// The submission summary for an assignment.
/// </summary>
public record SubmissionSummary
{
    /// <summary>
    /// The identifier of the course.
    /// </summary>
    [JsonIgnore]
    public ulong CourseId { get; init; }

    /// <summary>
    /// The identifier of the assignment.
    /// </summary>
    [JsonIgnore]
    public ulong AssignmentId { get; init; }

    /// <summary>
    /// The number of graded assignments.
    /// </summary>
    public int Graded { get; init; }

    /// <summary>
    /// The number of not submitted assignments
    /// </summary>
    [JsonPropertyName("not_submitted")]
    public int NotSubmitted { get; init; }

    /// <summary>
    /// The total number of possible submissions.
    /// </summary>
    [JsonIgnore]
    public int Total => Graded + Ungraded + NotSubmitted;

    /// <summary>
    /// The number of ungraded assignments.
    /// </summary>
    public int Ungraded { get; init; }
}