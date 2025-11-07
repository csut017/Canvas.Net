namespace Canvas.Core.Entities;

/// <summary>
/// A Canvas user.
/// </summary>
public record User
    : EntityWithIdAndName
{
    /// <summary>
    /// The user's organisational ID.
    /// </summary>
    [JsonPropertyName("sis_user_id")]
    public string? OrganisationId { get; init; }

    /// <summary>
    /// The user's email address.
    /// </summary>
    public string? Email { get; init; }

    ///// <summary>
    ///// The enrolments for the user.
    ///// </summary>
    //public List<Enrolment>? Enrollments { get; init; }

    /// <summary>
    /// The ids of the groups the user is in.
    /// </summary>
    [JsonPropertyName("group_ids")]
    public ulong[] GroupIds { get; init; } = [];

    /// <summary>
    /// The user's display name.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string? NameDisplay { get; init; }

    /// <summary>
    /// The user's sortable name.
    /// </summary>
    [JsonPropertyName("sortable_name")]
    public string? NameSortable { get; init; }

    /// <summary>
    /// The user's first name.
    /// </summary>
    [JsonPropertyName("first_name")]
    public string? NamePersonal { get; init; }

    /// <summary>
    /// The user's family name.
    /// </summary>
    [JsonPropertyName("last_name")]
    public string? NameFamily { get; init; }

    /// <summary>
    /// The user's login ID.
    /// </summary>
    [JsonPropertyName("login_id")]
    public string? LoginId { get; init; }

    /// <summary>
    /// Returns the display name of the user.
    /// </summary>
    /// <returns>The person's display name.</returns>
    public string GetDisplayName()
    {
        return string.IsNullOrEmpty(NameDisplay)
            ? Name
            : NameDisplay;
    }
}