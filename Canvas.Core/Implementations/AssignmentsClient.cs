using Canvas.Core.Clients;
using Canvas.Core.Entities;
using Canvas.Core.Settings;
using CommunityToolkit.Diagnostics;
using Serilog;
using System.Runtime.CompilerServices;

namespace Canvas.Core.Implementations;

/// <summary>
/// Default implementation of <see cref="IAssignments"/>.
/// </summary>
internal class AssignmentsClient
    : IAssignments
{
    private readonly IConnection _connection;
    private readonly ILogger? _logger;

    /// <summary>
    /// Initialises a new <see cref="ICurrentUser"/> instance.
    /// </summary>
    /// <param name="connection">The underlying connection.</param>
    /// <param name="logger">An optional logger.</param>
    public AssignmentsClient(IConnection connection, ILogger? logger = null)
    {
        Guard.IsNotNull(connection);
        _connection = connection;
        _logger = logger?.ForContext<AssignmentsClient>();
    }

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
    public Task<AssignmentDate> AddAssignmentOverrideForSection(ulong courseId, ulong assignmentId, ulong sectionId, DateTime? whenDue,
        DateTime? whenUnlock, DateTime? whenLock, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

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
    public async Task<Submission> AddComment(
        ulong courseId,
        ulong assignmentId,
        ulong studentId,
        string comment,
        FileUpload? file,
        CancellationToken cancellationToken = default)
    {
        var url = $"/api/v1/courses/{courseId}/assignments/{assignmentId}/submissions/{studentId}";

        // Generate the values
        var values = Parameters.New()
            .Add("comment[text_comment]", comment);
        if (file != null)
        {
            _logger?.Debug("Uploading comment file for student {studentId} for {assignmentId} in {courseId}", studentId, assignmentId, courseId);
            var fileUrl = $"/api/v1/courses/{courseId}/assignments/{assignmentId}/submissions/{studentId}/comments/files";
            var attachmentInfo = await _connection.UploadFile<Submission>(
                fileUrl,
                file.GenerateUploadArgs(),
                file,
                cancellationToken);
            values.Add("comment[file_ids][]", attachmentInfo.Id.ToString());
        }

        // Send the data and check the response
        _logger?.Debug("Adding comment to student {studentId} for {assignmentId} in {courseId}", studentId, assignmentId, courseId);
        var response = await _connection.PutForm<Submission>(
            url,
            values,
            true,
            cancellationToken);
        Guard.IsNotNull(response);
        return response with { CourseId = courseId };
    }

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
    public Task<Submission> AddComment(Course course, Assignment assignment, User student, string comment, FileUpload? file,
        CancellationToken cancellationToken = default)
    {
        return AddComment(
            course.Id,
            assignment.Id,
            student.Id,
            comment,
            file,
            cancellationToken);
    }

    /// <summary>
    /// Creates a new assignment.
    /// </summary>
    /// <param name="course">The <see cref="Course"/> to create the assignment in.</param>
    /// <param name="assignment">The assignment details.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A new <see cref="Assignment"/> instance.</returns>
    public Task<Assignment> Create(Course course, Assignment assignment, CancellationToken cancellationToken = default)
    {
        return Create(course.Id, assignment, cancellationToken);
    }

    /// <summary>
    /// Creates a new assignment.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignment">The assignment details.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A new <see cref="Assignment"/> instance.</returns>
    public async Task<Assignment> Create(ulong courseId, Assignment assignment, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(assignment.Name)) throw new ClientException("Assignments must have a name - cannot be null or empty");
        _logger?.Debug("Creating new assignment in course {courseId} with name {name}", courseId, assignment.Name);
        var item = await _connection.PostJson<Assignment>(
            $"/api/v1/courses/{courseId}/assignments",
            new { assignment },
            true,
            cancellationToken);
        if (item == null) throw new ClientException("No assignment returned from Canvas");
        return item with { CourseId = courseId };
    }

    /// <summary>
    /// Downloads the contents of a submission.
    /// </summary>
    /// <param name="submission">The <see cref="SubmissionFile"/> to download.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A string containing the contents of the submission.</returns>
    public async Task<string> DownloadSubmissionAsString(SubmissionFile submission, CancellationToken cancellationToken = default)
    {
        _logger?.Debug("Downloading submission from {url}", submission.Url);
        var response = await _connection.Get(
            submission.Url,
            true,
            cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return body;
    }

    /// <summary>
    /// Downloads a submission to a file path.
    /// </summary>
    /// <param name="submission">The <see cref="SubmissionFile"/> to download.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <param name="path">The path to download the file to.</param>
    public async Task DownloadSubmissionToFile(SubmissionFile submission, string path, CancellationToken cancellationToken = default)
    {
        await using var stream = File.Create(path);
        await DownloadSubmissionToStream(
            submission,
            stream,
            cancellationToken);
    }

    /// <summary>
    /// Downloads the contents of a submission.
    /// </summary>
    /// <param name="submission">The <see cref="SubmissionFile"/> to download.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <param name="stream">The <see cref="Stream"/> to save the contents to.</param>
    public async Task DownloadSubmissionToStream(SubmissionFile submission, Stream stream, CancellationToken cancellationToken = default)
    {
        _logger?.Debug("Downloading submission from {url}", submission.Url);
        var response = await _connection.Get(
            submission.Url,
            true,
            cancellationToken);
        await response.Content.CopyToAsync(stream, cancellationToken);
    }

    /// <summary>
    /// Lists the assignments for a course.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="opts">The list options.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Assignment}"/> containing the assignments for the course.</returns>
    public async IAsyncEnumerable<Assignment> ListForCourse(ulong courseId, AssignmentList? opts = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        opts ??= new AssignmentList();

        _logger?.Debug("Listing assignments for course with id {courseId}", courseId);
        await foreach (var item in _connection.List<Assignment>(
            $"/api/v1/courses/{courseId}/assignments",
            opts,
            cancellationToken))
        {
            yield return item with
            {
                CourseId = courseId,
            };
        }
    }

    /// <summary>
    /// Lists the assignments for a course.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course.</param>
    /// <param name="opts">The list options.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Assignment}"/> containing the assignments for the course.</returns>
    public IAsyncEnumerable<Assignment> ListForCourse(Course course, AssignmentList? opts = null,
        CancellationToken cancellationToken = default)
    {
        return ListForCourse(course.Id, opts, cancellationToken);
    }

    /// <summary>
    /// Lists the override dates for an assignment in a course.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance.</param>
    /// <param name="opts">The list options.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{AssignmentDate}"/> containing the override dates for an assignment.</returns>
    public IAsyncEnumerable<AssignmentDate> ListOverrideDates(Course course, Assignment assignment, List? opts = null,
        CancellationToken cancellationToken = default)
    {
        return ListOverrideDates(course.Id, assignment.Id, opts, cancellationToken);
    }

    /// <summary>
    /// Lists the override dates for an assignment in a course.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="opts">The list options.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{AssignmentDate}"/> containing the override dates for an assignment.</returns>
    public async IAsyncEnumerable<AssignmentDate> ListOverrideDates(ulong courseId, ulong assignmentId, List? opts = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        opts ??= new List();
        _logger?.Debug("Listing override dates for assignment {assignmentId} in course with id {courseId}", assignmentId, courseId);
        await foreach (var item in _connection.List<AssignmentDate>(
            $"/api/v1/courses/{courseId}/assignments/{assignmentId}/overrides",
            opts,
            cancellationToken))
        {
            yield return item with
            {
                AssignmentId = assignmentId,
                CourseId = courseId
            };
        }
    }

    /// <summary>
    /// Lists the peer reviews for an assignment in a course.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="opts">The list options.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{PeerReview}"/> containing the peer reviews for an assignment.</returns>
    public async IAsyncEnumerable<PeerReview> ListPeerReviews(ulong courseId, ulong assignmentId, List? opts = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        opts ??= new List();
        _logger?.Debug("Listing peer reviews for assignment {assignmentId} in course with id {courseId}", assignmentId, courseId);
        await foreach (var item in _connection.List<PeerReview>(
                           $"/api/v1/courses/{courseId}/assignments/{assignmentId}/peer_reviews",
                           opts,
                           cancellationToken))
        {
            yield return item with
            {
                AssignmentId = assignmentId,
                CourseId = courseId
            };
        }
    }

    /// <summary>
    /// Lists the peer reviews for an assignment in a course.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance.</param>
    /// <param name="opts">The list options.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{PeerReview}"/> containing the peer reviews for an assignment.</returns>
    public IAsyncEnumerable<PeerReview> ListPeerReviews(Course course, Assignment assignment, List? opts = null,
        CancellationToken cancellationToken = default)
    {
        return ListPeerReviews(course.Id, assignment.Id, opts, cancellationToken);
    }

    /// <summary>
    /// Lists the submissions for an assignment in a course.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="opts">The list options.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Submission}"/> containing the submissions for an assignment.</returns>
    public async IAsyncEnumerable<Submission> ListSubmissions(ulong courseId, ulong assignmentId, SubmissionList? opts = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        opts ??= new SubmissionList();
        if (!opts.Options.HasFlag(SubmissionInclude.User)) opts = opts with { Options = opts.Options | SubmissionInclude.User };
        _logger?.Debug("Listing submissions for assignment {assignmentId} in course with id {courseId}", assignmentId, courseId);
        await foreach (var item in _connection.List<Submission>(
                           $"/api/v1/courses/{courseId}/assignments/{assignmentId}/submissions",
                           opts,
                           cancellationToken))
        {
            yield return item with
            {
                AssignmentId = assignmentId,
                CourseId = courseId
            };
        }
    }

    /// <summary>
    /// Lists the submissions for an assignment in a course.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance.</param>
    /// <param name="opts">The list options.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Submission}"/> containing the submissions for an assignment.</returns>
    public IAsyncEnumerable<Submission> ListSubmissions(Course course, Assignment assignment, SubmissionList? opts = null,
        CancellationToken cancellationToken = default)
    {
        return ListSubmissions(course.Id, assignment.Id, opts, cancellationToken);
    }

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
    public Task<Submission> MarkSubmission(ulong courseId, ulong assignmentId, ulong studentId, double? mark, string? comment,
        SubmissionRubric? rubric = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

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
    public Task<Submission> MarkSubmission(Course course, Assignment assignment, ulong studentId, double? mark, string? comment,
        SubmissionRubric? rubric = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

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
    public Task<Submission> MarkSubmission(Course course, Assignment assignment, User student, double? mark, string? comment,
        SubmissionRubric? rubric = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retrieves the details on an assignment.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="opts">Any options for retrieving the assignment.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Course"/> instance if found on Canvas; <c>null</c> otherwise.</returns>
    public async Task<Assignment?> Retrieve(ulong courseId, ulong assignmentId, AssignmentItem? opts = null,
        CancellationToken cancellationToken = default)
    {
        opts ??= new AssignmentItem();
        _logger?.Debug("Retrieving assignment {assignmentId} in course with id {courseId}", assignmentId, courseId);
        var assignment = await _connection.Retrieve<Assignment>(
            $"/api/v1/courses/{courseId}/assignments/{assignmentId}",
            opts.ToParameters(),
            cancellationToken);
        return assignment == null
            ? null
            : assignment with { CourseId = courseId };
    }

    /// <summary>
    /// Retrieves the details on an assignment.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance to retrieve the details for.</param>
    /// <param name="opts">Any options for retrieving the assignment.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="Assignment"/> instance if found on Canvas; <c>null</c> otherwise.</returns>
    public Task<Assignment?> Retrieve(Course course, Assignment assignment, AssignmentItem? opts = null,
        CancellationToken cancellationToken = default)
    {
        return Retrieve(course.Id, assignment.Id, opts, cancellationToken);
    }

    /// <summary>
    /// Retrieves an assignment submission for a single user.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="opts">Any options for retrieving the submission.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Submission"/> instance if found, <c>null</c> otherwise.</returns>
    public Task<Submission?> RetrieveSubmission(ulong courseId, ulong assignmentId, ulong userId, SubmissionList? opts = null,
        CancellationToken cancellationToken = default)
    {
        opts ??= new();
        _logger?.Debug("Retrieving submission for assignment {assignmentId} in course with id {courseId} by student {userId}", assignmentId, courseId, userId);
        return _connection.Retrieve<Submission>(
            $"/api/v1/courses/{courseId}/assignments/{assignmentId}/submissions/{userId}",
            opts.ToParameters(),
            cancellationToken);
    }

    /// <summary>
    /// Retrieves an assignment submission for a single user.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance.</param>
    /// <param name="user">The user.</param>
    /// <param name="opts">Any options for retrieving the submission.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="Submission"/> instance if found, <c>null</c> otherwise.</returns>
    public Task<Submission?> RetrieveSubmission(Course course, Assignment assignment, User user, SubmissionList? opts = null,
        CancellationToken cancellationToken = default)
    {
        return RetrieveSubmission(course.Id, assignment.Id, user.Id, opts, cancellationToken);
    }

    /// <summary>
    /// Lists the submissions for an assignment in a course.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Submission}"/> containing the submissions for an assignment.</returns>
    public Task<SubmissionSummary> RetrieveSubmissionSummary(Course course, Assignment assignment, CancellationToken cancellationToken = default)
    {
        return RetrieveSubmissionSummary(course.Id, assignment.Id, cancellationToken);
    }

    /// <summary>
    /// Lists the submissions for an assignment in a course.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{Submission}"/> containing the submissions for an assignment.</returns>
    public async Task<SubmissionSummary> RetrieveSubmissionSummary(ulong courseId, ulong assignmentId, CancellationToken cancellationToken = default)
    {
        _logger?.Debug("Retrieving submission summary for assignment {assignmentId} in course with id {courseId}", assignmentId, courseId);
        var summary = await _connection.Retrieve<SubmissionSummary>(
            $"/api/v1/courses/{courseId}/assignments/{assignmentId}/submission_summary",
            [],
            cancellationToken);
        if (summary == null) throw new ClientException("No summary returned from Canvas");
        return summary with
        {
            AssignmentId = assignmentId,
            CourseId = courseId
        };
    }

    /// <summary>
    /// Updates an existing assignment.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="assignment">The assignment details.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A new <see cref="Assignment"/> instance.</returns>
    public async Task<Assignment> Update(ulong courseId, ulong assignmentId, Assignment assignment, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(assignment.Name)) throw new ClientException("Assignments must have a name - cannot be null or empty");
        _logger?.Debug("Updating existing assignment in course {courseId} with name {name} [{assignmentId}]", courseId, assignment.Name, assignmentId);
        var item = await _connection.PutJson<Assignment>(
            $"/api/v1/courses/{courseId}/assignments/{assignmentId}",
            new { assignment },
            true,
            cancellationToken);
        if (item == null) throw new ClientException("No assignment returned from Canvas");
        return item with { CourseId = courseId };
    }

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
    public Task<Assignment> Update(Course? course, Assignment assignment, CancellationToken cancellationToken = default)
    {
        var courseId = course?.Id ?? assignment.CourseId;
        return Update(courseId, assignment.Id, assignment, cancellationToken);
    }

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
    public Task<AssignmentDate> UpdateAssignmentOverride(ulong courseId, ulong assignmentId, ulong overrideId, DateTime? whenDue,
        DateTime? whenUnlock, DateTime? whenLock, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

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
    public Task<Submission> UpdateSubmissionLateness(ulong courseId, ulong assignmentId, ulong studentId, LatePolicyStatus status,
        int? lateness = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

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
    public Task<Submission> UpdateSubmissionLateness(Course course, Assignment assignment, ulong studentId, LatePolicyStatus status,
        int? lateness = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

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
    public Task<Submission> UpdateSubmissionLateness(Course course, Assignment assignment, User student, LatePolicyStatus status,
        int? lateness = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Uploads a submission for a student.
    /// </summary>
    /// <param name="courseId">The identifier of the course.</param>
    /// <param name="assignmentId">The identifier of the assignment.</param>
    /// <param name="studentId">The identifier of the student.</param>
    /// <param name="file">The file to upload.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>The submission details.</returns>
    public Task<Submission> UploadSubmission(ulong courseId, ulong assignmentId, ulong studentId, FileUpload file,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Uploads a submission for a student.
    /// </summary>
    /// <param name="course">A <see cref="Course"/> instance for the course that owns the assignment.</param>
    /// <param name="assignment">A <see cref="Assignment"/> instance.</param>
    /// <param name="student">A <see cref="User"/> containing the student details to upload the assignment for.</param>
    /// <param name="file">The file to upload.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>The submission details.</returns>
    public Task<Submission> UploadSubmission(Course course, Assignment assignment, User student, FileUpload file,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}