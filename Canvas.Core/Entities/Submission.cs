namespace Canvas.Core.Entities;

/// <summary>
/// A submission for an assignment.
/// </summary>
public record Submission
    : EntityWithId, ICourseItem
{
    /// <summary>
    /// The attached files for the submission.
    /// </summary>
    public List<SubmissionFile>? Attachments { get; init; }

    /// <summary>
    /// The associated comments.
    /// </summary>
    [JsonPropertyName("submission_comments")]
    public List<SubmissionComment>? Comments { get; init; }

    /// <summary>
    /// The amount of time, in seconds, that an submission is late by.
    /// </summary>
    [JsonPropertyName("seconds_late")]
    public int? LatenessInSeconds { get; init; }

    /// <summary>
    /// The status of the submission in relation to the late policy.
    /// </summary>
    [JsonPropertyName("late_policy_status")]
    public LatePolicyStatus? LatePolicyStatus { get; init; }

    /// <summary>
    /// The assigned rubric marks.
    /// </summary>
    [JsonPropertyName("rubric_assessment")]
    public SubmissionRubric? Rubric { get; init; }

    /// <summary>
    /// The final score for the submission.
    /// </summary>
    public double? Score { get; init; }

    /// <summary>
    /// The amount of points deducted due to late or missing assignment policy.
    /// </summary>
    [JsonPropertyName("points_deducted")]
    public double? ScorePenalty { get; init; }

    /// <summary>
    /// The user who made the submission.
    /// </summary>
    public User? User { get; init; }

    /// <summary>
    /// A flag indicating whether the submission was late.
    /// </summary>
    [JsonPropertyName("late")]
    public bool WasSubmittedLate { get; init; }

    /// <summary>
    /// The date and time of the submission.
    /// </summary>
    [JsonPropertyName("submitted_at")]
    public DateTime? WhenSubmitted { get; init; }

    /// <summary>
    /// The current state of the submission.
    /// </summary>
    [JsonPropertyName("workflow_state")]
    public string? WorkflowState { get; init; }

    /// <summary>
    /// The identifier of the owning course.
    /// </summary>
    [JsonIgnore]
    public ulong CourseId { get; init; }

    /// <summary>
    /// The identifier of the assignment.
    /// </summary>
    [JsonIgnore]
    public ulong AssignmentId { get; init; }
}