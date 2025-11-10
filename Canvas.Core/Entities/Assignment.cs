namespace Canvas.Core.Entities;

/// <summary>
/// The details of an assignment.
/// </summary>
public record Assignment
    : EntityWithIdAndName, ICourseItem
{
    /// <summary>
    /// The allowed file extensions.
    /// </summary>
    [JsonPropertyName("allowed_extensions")]
    public string[]? AllowedExtensions { get; init; }

    /// <summary>
    /// The associated dates for the assignment.
    /// </summary>
    /// <remarks>
    /// For a large number of due dates, this property will be empty. Instead, check <see cref="NumberOfDates"/> to see if all dates have been
    /// returned.
    /// </remarks>
    [JsonPropertyName("all_dates")]
    public List<AssignmentDate>? Dates { get; init; }

    /// <summary>
    /// The description of the assignment.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The group category ID for group assignments.
    /// </summary>
    [JsonPropertyName("group_category_id")]
    public ulong? GroupCategoryId { get; init; }

    /// <summary>
    /// The group that the assignment is part of.
    /// </summary>
    [JsonPropertyName("assignment_group_id")]
    public ulong GroupId { get; init; }

    /// <summary>
    /// A flag indicating whether the assignment is published.
    /// </summary>
    [JsonPropertyName("published")]
    public bool IsPublished { get; init; }

    /// <summary>
    /// The number of dates for this assignment.
    /// </summary>
    [JsonPropertyName("all_dates_count")]
    public int NumberOfDates { get; init; }

    /// <summary>
    /// A flag indicating whether the grade for this assignment should be omitted from the final grade.
    /// </summary>
    [JsonPropertyName("omit_from_final_grade")]
    public bool OmitFromFinalGrade { get; init; }

    /// <summary>
    /// The maximum number of points possible for this assignment.
    /// </summary>
    [JsonPropertyName("points_possible")]
    public double? Points { get; init; }

    /// <summary>
    /// The position within the course or assignment group.
    /// </summary>
    public int Position { get; init; }

    /// <summary>
    /// The associated marking rubric.
    /// </summary>
    public List<RubricItem>? Rubric { get; init; }

    /// <summary>
    /// The types of submission allowed.
    /// </summary>
    [JsonPropertyName("submission_types")]
    public string[] SubmissionTypes { get; init; } = [];

    /// <summary>
    /// The URL to the assignment's web page.
    /// </summary>
    [JsonPropertyName("html_url")]
    public string? Url { get; init; }

    /// <summary>
    /// The date and time when the assignment is available from.
    /// </summary>
    [JsonPropertyName("unlock_at")]
    public DateTime? WhenAvailableFrom { get; init; }

    /// <summary>
    /// The date and time when the assignment is available to.
    /// </summary>
    [JsonPropertyName("lock_at")]
    public DateTime? WhenAvailableTo { get; init; }

    /// <summary>
    /// The date and time when the assignment is due.
    /// </summary>
    [JsonPropertyName("due_at")]
    public DateTime? WhenDue { get; init; }

    /// <summary>
    /// The identifier of the owning course.
    /// </summary>
    public ulong CourseId { get; init; }
}