using Canvas.Core.Entities;
using Canvas.Core.Settings;

namespace Canvas.Core.Clients;

/// <summary>
/// Provides access to the assignments-related functionality for courses.
/// </summary>
public interface IAssignments
{
    /// <summary>
    /// Adds an assignment override for a section in an assignment.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="sectionId">The identifier of the section.</param>
    /// <param name="whenDue">When the assignment is due for the section.</param>
    /// <param name="whenUnlock">When the assignment is unlocked for the section.</param>
    /// <param name="whenLock">When the assignment is locked for the section.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A new <see cref="AssignmentDate"/> containing the results of the add operation.</returns>
    Task<AssignmentDate> AddAssignmentOverrideForSection(ulong courseId, ulong assignmentId, ulong sectionId, DateTime? whenDue, DateTime? whenUnlock, DateTime? whenLock, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a submission for an assignment.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="studentId">The identifier of the student.</param>
    /// <param name="comment">An optional comment for the submission.</param>
    /// <param name="file">An optional file to associate with the comment.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Submission"/> instance containing the result from Canvas.</returns>
    Task<Submission> AddComment(ulong courseId, ulong assignmentId, ulong studentId, string comment, FileUpload? file, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a submission for an assignment.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance.</param>
    /// <param name="student"></param>
    /// <param name="comment">An optional comment for the submission.</param>
    /// <param name="file">An optional file to associate with the comment.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Submission"/> instance containing the result from Canvas.</returns>
    Task<Submission> AddComment(Course course, Assignment assignment, User student, string comment, FileUpload? file, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new assignment.
    /// </summary>
    /// <param name="course">The <see cref="Course"/> to create the assignment in.</param>
    /// <param name="assignment">The assignment details.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A new <see cref="Assignment"/> instance.</returns>
    Task<Assignment> Create(Course course, Assignment assignment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new assignment.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignment">The assignment details.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A new <see cref="Assignment"/> instance.</returns>
    Task<Assignment> Create(ulong courseId, Assignment assignment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the contents of a submission.
    /// </summary>
    /// <param name="submission">The <see cref="SubmissionFile"/> to download.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A string containing the contents of the submission.</returns>
    Task<string> DownloadSubmissionAsString(SubmissionFile submission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the contents of a submission.
    /// </summary>
    /// <param name="submission">The <see cref="SubmissionFile"/> to download.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <param name="stream">The <see cref="Stream"/> to save the contents to.</param>
    Task DownloadSubmissionToStream(SubmissionFile submission, Stream stream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a submission to a file path.
    /// </summary>
    /// <param name="submission">The <see cref="SubmissionFile"/> to download.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <param name="path">The path to download the file to.</param>
    Task DownloadSubmissionToFile(SubmissionFile submission, string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the assignments for a course.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="opts">The list options.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Assignment}"/> containing the assignments for the course.</returns>
    IAsyncEnumerable<Assignment> ListForCourse(ulong courseId, AssignmentList? opts = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the assignments for a course.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course.</param>
    /// <param name="opts">The list options.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Assignment}"/> containing the assignments for the course.</returns>
    IAsyncEnumerable<Assignment> ListForCourse(Course course, AssignmentList? opts = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the override dates for an assignment in a course.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance.</param>
    /// <param name="opts">The list options.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{AssignmentDate}"/> containing the override dates for an assignment.</returns>
    IAsyncEnumerable<AssignmentDate> ListOverrideDates(Course course, Assignment assignment, List? opts = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the override dates for an assignment in a course.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="opts">The list options.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{AssignmentDate}"/> containing the override dates for an assignment.</returns>
    IAsyncEnumerable<AssignmentDate> ListOverrideDates(ulong courseId, ulong assignmentId, List? opts = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the peer reviews for an assignment in a course.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="opts">The list options.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{PeerReview}"/> containing the peer reviews for an assignment.</returns>
    IAsyncEnumerable<PeerReview> ListPeerReviews(ulong courseId, ulong assignmentId, List? opts = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the peer reviews for an assignment in a course.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance.</param>
    /// <param name="opts">The list options.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{PeerReview}"/> containing the peer reviews for an assignment.</returns>
    IAsyncEnumerable<PeerReview> ListPeerReviews(Course course, Assignment assignment, List? opts = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the submissions for an assignment in a course.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="opts">The list options.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Submission}"/> containing the submissions for an assignment.</returns>
    IAsyncEnumerable<Submission> ListSubmissions(ulong courseId, ulong assignmentId, SubmissionList? opts = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the submissions for an assignment in a course.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance.</param>
    /// <param name="opts">The list options.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Submission}"/> containing the submissions for an assignment.</returns>
    IAsyncEnumerable<Submission> ListSubmissions(Course course, Assignment assignment, SubmissionList? opts = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a submission for an assignment.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="studentId">The identifier of the student.</param>
    /// <param name="mark">An optional mark to assign.</param>
    /// <param name="comment">An optional comment for the submission.</param>
    /// <param name="rubric">An optional marked rubric.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Submission"/> instance containing the result from Canvas.</returns>
    Task<Submission> MarkSubmission(ulong courseId, ulong assignmentId, ulong studentId, double? mark, string? comment, SubmissionRubric? rubric = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a submission for an assignment.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance.</param>
    /// <param name="studentId">The identifier of the student.</param>
    /// <param name="mark">An optional mark to assign.</param>
    /// <param name="comment">An optional comment for the submission.</param>
    /// <param name="rubric">An optional marked rubric.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Submission"/> instance containing the result from Canvas.</returns>
    Task<Submission> MarkSubmission(Course course, Assignment assignment, ulong studentId, double? mark, string? comment, SubmissionRubric? rubric = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a submission for an assignment.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance.</param>
    /// <param name="student"></param>
    /// <param name="mark">An optional mark to assign.</param>
    /// <param name="comment">An optional comment for the submission.</param>
    /// <param name="rubric">An optional marked rubric.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Submission"/> instance containing the result from Canvas.</returns>
    Task<Submission> MarkSubmission(Course course, Assignment assignment, User student, double? mark, string? comment, SubmissionRubric? rubric = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the details on an assignment.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="opts">Any options for retrieving the assignment.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Course"/> instance if found on Canvas; <c>null</c> otherwise.</returns>
    Task<Assignment?> Retrieve(ulong courseId, ulong assignmentId, AssignmentItem? opts = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the details on an assignment.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance to retrieve the details for.</param>
    /// <param name="opts">Any options for retrieving the assignment.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="Assignment"/> instance if found on Canvas; <c>null</c> otherwise.</returns>
    Task<Assignment?> Retrieve(Course course, Assignment assignment, AssignmentItem? opts = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an assignment submission for a single user.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="opts">Any options for retrieving the submission.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Submission"/> instance if found, <c>null</c> otherwise.</returns>
    Task<Submission?> RetrieveSubmission(ulong courseId, ulong assignmentId, ulong userId, SubmissionList? opts = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an assignment submission for a single user.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance.</param>
    /// <param name="user">The user.</param>
    /// <param name="opts">Any options for retrieving the submission.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Submission"/> instance if found, <c>null</c> otherwise.</returns>
    Task<Submission?> RetrieveSubmission(Course course, Assignment assignment, User user, SubmissionList? opts = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the submissions for an assignment in a course.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Submission}"/> containing the submissions for an assignment.</returns>
    Task<SubmissionSummary> RetrieveSubmissionSummary(Course course, Assignment assignment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists the submissions for an assignment in a course.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Submission}"/> containing the submissions for an assignment.</returns>
    Task<SubmissionSummary> RetrieveSubmissionSummary(ulong courseId, ulong assignmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing assignment.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="assignment">The assignment details.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A new <see cref="Assignment"/> instance.</returns>
    Task<Assignment> Update(ulong courseId, ulong assignmentId, Assignment assignment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing assignment.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">The assignment details.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A new <see cref="Assignment"/> instance.</returns>
    /// <remarks>
    /// If <c>course</c> is <c>null</c>, then the CourseId must be set in <c>assignment</c>.
    /// </remarks>
    Task<Assignment> Update(Course? course, Assignment assignment, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an assignment override in an assignment.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="overrideId">The identifier of the override.</param>
    /// <param name="whenDue">When the assignment is due for the section.</param>
    /// <param name="whenUnlock">When the assignment is unlocked for the section.</param>
    /// <param name="whenLock">When the assignment is locked for the section.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A new <see cref="AssignmentDate"/> containing the results of the add operation.</returns>
    Task<AssignmentDate> UpdateAssignmentOverride(ulong courseId, ulong assignmentId, ulong overrideId, DateTime? whenDue, DateTime? whenUnlock, DateTime? whenLock, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the lateness details for a submission for an assignment.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="studentId">The identifier of the student.</param>
    /// <param name="status">The new late status.</param>
    /// <param name="lateness">The new value for the number of seconds late.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Submission"/> instance containing the result from Canvas.</returns>
    Task<Submission> UpdateSubmissionLateness(ulong courseId, ulong assignmentId, ulong studentId, LatePolicyStatus status, int? lateness = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the lateness details for a submission for an assignment.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance.</param>
    /// <param name="studentId">The identifier of the student.</param>
    /// <param name="status">The new late status.</param>
    /// <param name="lateness">The new value for the number of seconds late.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Submission"/> instance containing the result from Canvas.</returns>
    Task<Submission> UpdateSubmissionLateness(Course course, Assignment assignment, ulong studentId, LatePolicyStatus status, int? lateness = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the lateness details for a submission for an assignment.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance.</param>
    /// <param name="student"></param>
    /// <param name="status">The new late status.</param>
    /// <param name="lateness">The new value for the number of seconds late.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Submission"/> instance containing the result from Canvas.</returns>
    Task<Submission> UpdateSubmissionLateness(Course course, Assignment assignment, User student, LatePolicyStatus status, int? lateness = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a submission for a student.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="studentId">The identifier of the student.</param>
    /// <param name="file">The file to upload.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>The submission details.</returns>
    Task<Submission> UploadSubmission(ulong courseId, ulong assignmentId, ulong studentId, FileUpload file, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a submission for a student.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance.</param>
    /// <param name="student">A <see cref="User"/> containing the student details to upload the assignment for.</param>
    /// <param name="file">The file to upload.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>The submission details.</returns>
    Task<Submission> UploadSubmission(Course course, Assignment assignment, User student, FileUpload file, CancellationToken cancellationToken = default);
}