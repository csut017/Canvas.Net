namespace Canvas.Core.Entities;

/// <summary>
/// A date associated with an assignment.
/// </summary>
[DebuggerDisplay($"{{{nameof(SetType)}}}: {{{nameof(Name)}}}")]
public record AssignmentDate
    : EntityWithId
{
    /// <summary>
    /// The ID of the group that these dates are for.
    /// </summary>
    [JsonPropertyName("group_id")]
    public ulong? GroupId { get; init; }

    /// <summary>
    /// A flag indicating whether these are the base dates for the assignment.
    /// </summary>
    [JsonPropertyName("base")]
    public bool IsBase { get; init; }

    /// <summary>
    /// The name of the date.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Name { get; init; }

    /// <summary>
    /// The ID of the section that these dates are for.
    /// </summary>
    [JsonPropertyName("course_section_id")]
    public ulong? SectionId { get; init; }

    /// <summary>
    /// The id of the associated set.
    /// </summary>
    [JsonPropertyName("set_id")]
    public ulong? SetId { get; init; }

    /// <summary>
    /// The set of date.
    /// </summary>
    [JsonPropertyName("set_type")]
    public string? SetType { get; init; }

    /// <summary>
    /// The IDs of the students that these dates are for.
    /// </summary>
    [JsonPropertyName("student_ids")]
    public List<ulong>? StudentIds { get; init; }

    /// <summary>
    /// The associated due date.
    /// </summary>
    [JsonPropertyName("due_at")]
    public DateTime? WhenDue { get; init; }

    /// <summary>
    /// The associated lock date.
    /// </summary>
    [JsonPropertyName("lock_at")]
    public DateTime? WhenLocked { get; init; }

    /// <summary>
    /// The associated unlock date.
    /// </summary>
    [JsonPropertyName("unlock_at")]
    public DateTime? WhenUnlocked { get; init; }
}