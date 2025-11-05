using System.Text.Json.Serialization;

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

    ///// <summary>
    ///// The options for listing users in a course.
    ///// </summary>
    //public class ListOptions
    //    : Models.ListOptions
    //{
    //    /// <summary>
    //    /// The enrolment states to filter by.
    //    /// </summary>
    //    public List<EnrolmentState> EnrolmentStates { get; } = [];

    //    /// <summary>
    //    /// The enrolment types to filter by.
    //    /// </summary>
    //    public List<EnrolmentType> EnrolmentTypes { get; } = [];

    //    /// <summary>
    //    /// Whether to include the enrollments or not.
    //    /// </summary>
    //    public bool IncludeEnrollments { get; init; }

    //    protected override void AppendParameters(Parameters parameters)
    //    {
    //        base.AppendParameters(parameters);
    //        foreach (var state in EnrolmentStates)
    //        {
    //            parameters.Add("enrollment_state[]", state.ToString().ToLowerInvariant());
    //        }

    //        foreach (var type in EnrolmentTypes)
    //        {
    //            parameters.Add("enrollment_type[]", type.ToString().ToLowerInvariant());
    //        }

    //        if (IncludeEnrollments) parameters.Add("include[]", "enrollments");
    //    }
    //}

    ///// <summary>
    ///// Options for listing students in a course.
    ///// </summary>
    //public class StudentListOptions
    //    : ListOptions
    //{
    //    /// <summary>
    //    /// Whether to include the Group IDs or not.
    //    /// </summary>
    //    public bool IncludeGroupIds { get; init; }

    //    protected override void AppendParameters(Parameters parameters)
    //    {
    //        EnrolmentTypes.Clear();
    //        EnrolmentTypes.Add(EnrolmentType.Student);
    //        base.AppendParameters(parameters);
    //        if (IncludeGroupIds) parameters.Add("include[]", "group_ids");
    //    }
    //}
}