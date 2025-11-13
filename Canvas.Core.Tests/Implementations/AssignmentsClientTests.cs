using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Canvas.Core.Entities;
using Canvas.Core.Implementations;
using Canvas.Core.Settings;
using FakeItEasy;

namespace Canvas.Core.Tests.Implementations;

[TestSubject(typeof(AssignmentsClient))]
public class AssignmentsClientTests
{
    [Fact]
    public async Task CreateViaEntityCallsUnderlyingConnection()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.PostJson<Assignment>(
                "/api/v1/courses/987/assignments",
                A<object>.Ignored,
                A<bool>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(new Assignment { Id = 149 }));
        var client = new AssignmentsClient(conn);
        var assignment = new Assignment
        {
            Id = 149,
            CourseId = 987,
            Name = "An assignment"
        };

        // Act
        var newItem = await client.Create(
            new Course { Id = 987 },
            assignment,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        newItem.ShouldSatisfyAllConditions(
            () => newItem.ShouldNotBeNull(),
            () => newItem.Id.ShouldBe(149UL),
            () => newItem.CourseId.ShouldBe(987UL)
        );
    }

    [Fact]
    public async Task CreateViaEntityChecksCanvasResponse()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.PostJson<Assignment>(
                "/api/v1/courses/987/assignments",
                A<object>.Ignored,
                A<bool>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult<Assignment>(null));
        var client = new AssignmentsClient(conn);
        var assignment = new Assignment { Name = "An assignment" };

        // Act
        var ex = await Assert.ThrowsAsync<ClientException>(
            async () => await client.Create(
                new Course { Id = 987 },
                assignment,
                cancellationToken: TestContext.Current.CancellationToken));

        // Assert
        ex.Message.ShouldBe("No assignment returned from Canvas");
    }

    [Fact]
    public async Task CreateViaEntityChecksName()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        var client = new AssignmentsClient(conn);
        var assignment = new Assignment { Name = string.Empty };

        // Act
        var ex = await Assert.ThrowsAsync<ClientException>(
            async () => await client.Create(
                new Course { Id = 987 },
                assignment,
                cancellationToken: TestContext.Current.CancellationToken));

        // Assert
        ex.Message.ShouldBe("Assignments must have a name - cannot be null or empty");
    }

    [Fact]
    public async Task CreateViaIdCallsUnderlyingConnection()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.PostJson<Assignment>(
                "/api/v1/courses/987/assignments",
                A<object>.Ignored,
                A<bool>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(new Assignment { Id = 149 }));
        var client = new AssignmentsClient(conn);
        var assignment = new Assignment
        {
            Id = 149,
            CourseId = 987,
            Name = "An assignment"
        };

        // Act
        var newItem = await client.Create(
            987,
            assignment,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        newItem.ShouldSatisfyAllConditions(
            () => newItem.ShouldNotBeNull(),
            () => newItem.Id.ShouldBe(149UL),
            () => newItem.CourseId.ShouldBe(987UL)
        );
    }

    [Fact]
    public async Task CreateViaIdChecksCanvasResponse()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.PostJson<Assignment>(
                "/api/v1/courses/987/assignments",
                A<object>.Ignored,
                A<bool>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult<Assignment>(null));
        var client = new AssignmentsClient(conn);
        var assignment = new Assignment { Name = "An assignment" };

        // Act
        var ex = await Assert.ThrowsAsync<ClientException>(
            async () => await client.Create(
                987,
                assignment,
                cancellationToken: TestContext.Current.CancellationToken));

        // Assert
        ex.Message.ShouldBe("No assignment returned from Canvas");
    }

    [Fact]
    public async Task CreateViaIdChecksName()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        var client = new AssignmentsClient(conn);
        var assignment = new Assignment { Name = string.Empty };

        // Act
        var ex = await Assert.ThrowsAsync<ClientException>(
            async () => await client.Create(
                987,
                assignment,
                cancellationToken: TestContext.Current.CancellationToken));

        // Assert
        ex.Message.ShouldBe("Assignments must have a name - cannot be null or empty");
    }

    [Fact]
    public async Task ListForCourseViaEntityCallsUnderlyingConnection()
    {
        // Arrange
        var data = new List<Assignment>
        {
            new() { Id = 1, },
            new() { Id = 2, },
        };
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.List<Assignment>(
                "/api/v1/courses/6/assignments",
                A<List>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(data.ToAsyncEnumerable());
        var client = new AssignmentsClient(conn);

        // Act
        var assignments = await client.ListForCourse(
                new Course { Id = 6 },
                cancellationToken: TestContext.Current.CancellationToken)
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        var expected = data.Select(a => a with { CourseId = 6 }).ToList();
        assignments.ShouldBe(expected);
    }

    [Fact]
    public async Task ListForCourseViaIdCallsUnderlyingConnection()
    {
        // Arrange
        var data = new List<Assignment>
        {
            new() { Id = 1, },
            new() { Id = 2, },
        };
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.List<Assignment>(
                "/api/v1/courses/6/assignments",
                A<List>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(data.ToAsyncEnumerable());
        var client = new AssignmentsClient(conn);

        // Act
        var assignments = await client.ListForCourse(
                6,
                cancellationToken: TestContext.Current.CancellationToken)
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        var expected = data.Select(a => a with { CourseId = 6 }).ToList();
        assignments.ShouldBe(expected);
    }

    [Fact]
    public async Task ListOverrideDatesViaEntityCallsUnderlyingConnection()
    {
        // Arrange
        var data = new List<AssignmentDate>
        {
            new() { Id = 1, },
            new() { Id = 2, },
        };
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.List<AssignmentDate>(
                "/api/v1/courses/6/assignments/78/overrides",
                A<List>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(data.ToAsyncEnumerable());
        var client = new AssignmentsClient(conn);

        // Act
        var assignments = await client.ListOverrideDates(
                new Course { Id = 6 },
                new Assignment { Id = 78 },
                cancellationToken: TestContext.Current.CancellationToken)
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        var expected = data.Select(a => a with { CourseId = 6, AssignmentId = 78 }).ToList();
        assignments.ShouldBe(expected);
    }

    [Fact]
    public async Task ListOverrideDatesViaIdCallsUnderlyingConnection()
    {
        // Arrange
        var data = new List<AssignmentDate>
        {
            new() { Id = 1, },
            new() { Id = 2, },
        };
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.List<AssignmentDate>(
                "/api/v1/courses/6/assignments/78/overrides",
                A<List>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(data.ToAsyncEnumerable());
        var client = new AssignmentsClient(conn);

        // Act
        var assignments = await client.ListOverrideDates(
                6,
                78,
                cancellationToken: TestContext.Current.CancellationToken)
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        var expected = data.Select(a => a with { CourseId = 6, AssignmentId = 78 }).ToList();
        assignments.ShouldBe(expected);
    }

    [Fact]
    public async Task ListPeerReviewsViaEntityCallsUnderlyingConnection()
    {
        // Arrange
        var data = new List<PeerReview>
        {
            new() { Id = 1, },
            new() { Id = 2, },
        };
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.List<PeerReview>(
                "/api/v1/courses/6/assignments/78/peer_reviews",
                A<List>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(data.ToAsyncEnumerable());
        var client = new AssignmentsClient(conn);

        // Act
        var assignments = await client.ListPeerReviews(
                new Course { Id = 6 },
                new Assignment { Id = 78 },
                cancellationToken: TestContext.Current.CancellationToken)
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        var expected = data.Select(a => a with { CourseId = 6, AssignmentId = 78 }).ToList();
        assignments.ShouldBe(expected);
    }

    [Fact]
    public async Task ListPeerReviewsViaIdCallsUnderlyingConnection()
    {
        // Arrange
        var data = new List<PeerReview>
        {
            new() { Id = 1, },
            new() { Id = 2, },
        };
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.List<PeerReview>(
                "/api/v1/courses/6/assignments/78/peer_reviews",
                A<List>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(data.ToAsyncEnumerable());
        var client = new AssignmentsClient(conn);

        // Act
        var assignments = await client.ListPeerReviews(
                6,
                78,
                cancellationToken: TestContext.Current.CancellationToken)
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        var expected = data.Select(a => a with { CourseId = 6, AssignmentId = 78 }).ToList();
        assignments.ShouldBe(expected);
    }

    [Fact]
    public async Task ListSubmissionsViaEntityCallsUnderlyingConnection()
    {
        // Arrange
        var data = new List<Submission>
        {
            new() { Id = 1, },
            new() { Id = 2, },
        };
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.List<Submission>(
                "/api/v1/courses/6/assignments/78/submissions",
                A<List>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(data.ToAsyncEnumerable());
        var client = new AssignmentsClient(conn);

        // Act
        var assignments = await client.ListSubmissions(
                new Course { Id = 6 },
                new Assignment { Id = 78 },
                cancellationToken: TestContext.Current.CancellationToken)
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        var expected = data.Select(a => a with { CourseId = 6, AssignmentId = 78 }).ToList();
        assignments.ShouldBe(expected);
    }

    [Fact]
    public async Task ListSubmissionsViaIdCallsUnderlyingConnection()
    {
        // Arrange
        var data = new List<Submission>
        {
            new() { Id = 1, },
            new() { Id = 2, },
        };
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.List<Submission>(
                "/api/v1/courses/6/assignments/78/submissions",
                A<List>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(data.ToAsyncEnumerable());
        var client = new AssignmentsClient(conn);

        // Act
        var assignments = await client.ListSubmissions(
                6,
                78,
                cancellationToken: TestContext.Current.CancellationToken)
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        var expected = data.Select(a => a with { CourseId = 6, AssignmentId = 78 }).ToList();
        assignments.ShouldBe(expected);
    }

    [Fact]
    public async Task RetrieveByEntityCallsUnderlyingConnection()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.Retrieve<Assignment>(
                "/api/v1/courses/987/assignments/1123",
                A<Parameters>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(new Assignment { Id = 1123 }));
        var client = new AssignmentsClient(conn);

        // Act
        var assignment = await client.Retrieve(
            new Course { Id = 987 },
            new Assignment { Id = 1123 },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        assignment.ShouldSatisfyAllConditions(
            () => assignment.ShouldNotBeNull(),
            () => assignment?.Id.ShouldBe(1123UL),
            () => assignment?.CourseId.ShouldBe(987UL)
        );
    }

    [Fact]
    public async Task RetrieveByIdCallsUnderlyingConnection()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.Retrieve<Assignment>(
                "/api/v1/courses/987/assignments/149",
                A<Parameters>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(new Assignment { Id = 149 }));
        var client = new AssignmentsClient(conn);

        // Act
        var assignment = await client.Retrieve(
            987,
            149,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        assignment.ShouldSatisfyAllConditions(
            () => assignment.ShouldNotBeNull(),
            () => assignment?.Id.ShouldBe(149UL),
            () => assignment?.CourseId.ShouldBe(987UL)
        );
    }

    [Fact]
    public async Task RetrieveSubmissionByEntityCallsUnderlyingConnection()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.Retrieve<Submission>(
                "/api/v1/courses/987/assignments/1123/submissions/11235",
                A<Parameters>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(new Submission { Id = 1123 }));
        var client = new AssignmentsClient(conn);

        // Act
        var submission = await client.RetrieveSubmission(
            new Course { Id = 987 },
            new Assignment { Id = 1123 },
            new User { Id = 11235 },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        submission.ShouldSatisfyAllConditions(
            () => submission.ShouldNotBeNull(),
            () => submission?.Id.ShouldBe(1123UL)
        );
    }

    [Fact]
    public async Task RetrieveSubmissionByIdCallsUnderlyingConnection()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.Retrieve<Submission>(
                "/api/v1/courses/987/assignments/149/submissions/11235",
                A<Parameters>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(new Submission { Id = 149 }));
        var client = new AssignmentsClient(conn);

        // Act
        var submission = await client.RetrieveSubmission(
            987,
            149,
            11235,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        submission.ShouldSatisfyAllConditions(
            () => submission.ShouldNotBeNull(),
            () => submission?.Id.ShouldBe(149UL)
        );
    }

    [Fact]
    public async Task RetrieveSubmissionSummaryByEntityCallsUnderlyingConnection()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.Retrieve<SubmissionSummary>(
                "/api/v1/courses/987/assignments/149/submission_summary",
                A<Parameters>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(new SubmissionSummary { NotSubmitted = 1, Graded = 2, Ungraded = 3 }));
        var client = new AssignmentsClient(conn);

        // Act
        var summary = await client.RetrieveSubmissionSummary(
            new Course { Id = 987 },
            new Assignment { Id = 149 },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        summary.ShouldSatisfyAllConditions(
            () => summary.CourseId.ShouldBe(987UL),
            () => summary.AssignmentId.ShouldBe(149UL),
            () => summary.Ungraded.ShouldBe(3),
            () => summary.NotSubmitted.ShouldBe(1),
            () => summary.Graded.ShouldBe(2)
        );
    }

    [Fact]
    public async Task RetrieveSubmissionSummaryByEntityHandlesMissingEntity()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.Retrieve<SubmissionSummary>(
                "/api/v1/courses/987/assignments/149/submission_summary",
                A<Parameters>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult<SubmissionSummary>(null));
        var client = new AssignmentsClient(conn);

        // Act
        var ex = await Assert.ThrowsAsync<ClientException>(async () => await client.RetrieveSubmissionSummary(
            new Course { Id = 987 },
            new Assignment { Id = 149 },
            cancellationToken: TestContext.Current.CancellationToken));

        // Assert
        ex.Message.ShouldBe("No summary returned from Canvas");
    }

    [Fact]
    public async Task RetrieveSubmissionSummaryByIdCallsUnderlyingConnection()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.Retrieve<SubmissionSummary>(
                "/api/v1/courses/987/assignments/149/submission_summary",
                A<Parameters>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(new SubmissionSummary { NotSubmitted = 1, Graded = 2, Ungraded = 3 }));
        var client = new AssignmentsClient(conn);

        // Act
        var summary = await client.RetrieveSubmissionSummary(
            987,
            149,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        summary.ShouldSatisfyAllConditions(
            () => summary.CourseId.ShouldBe(987UL),
            () => summary.AssignmentId.ShouldBe(149UL),
            () => summary.Ungraded.ShouldBe(3),
            () => summary.NotSubmitted.ShouldBe(1),
            () => summary.Graded.ShouldBe(2)
        );
    }

    [Fact]
    public async Task RetrieveSubmissionSummaryByIdHandlesMissingEntity()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.Retrieve<SubmissionSummary>(
                "/api/v1/courses/987/assignments/149/submission_summary",
                A<Parameters>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult<SubmissionSummary>(null));
        var client = new AssignmentsClient(conn);

        // Act
        var ex = await Assert.ThrowsAsync<ClientException>(async () => await client.RetrieveSubmissionSummary(
            987,
            149,
            cancellationToken: TestContext.Current.CancellationToken));

        // Assert
        ex.Message.ShouldBe("No summary returned from Canvas");
    }

    [Fact]
    public async Task UpdateCallsUnderlyingConnection()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.PutJson<Assignment>(
                "/api/v1/courses/987/assignments/149",
                A<object>.Ignored,
                A<bool>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(new Assignment { Id = 149 }));
        var client = new AssignmentsClient(conn);
        var assignment = new Assignment
        {
            Id = 149,
            CourseId = 987,
            Name = "An assignment"
        };

        // Act
        var updated = await client.Update(
            987,
            149,
            assignment,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        updated.ShouldSatisfyAllConditions(
            () => updated.ShouldNotBeNull(),
            () => updated.Id.ShouldBe(149UL),
            () => updated.CourseId.ShouldBe(987UL)
        );
    }

    [Fact]
    public async Task UpdateChecksCanvasResponse()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.PutJson<Assignment>(
                "/api/v1/courses/987/assignments/149",
                A<object>.Ignored,
                A<bool>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult<Assignment>(null));
        var client = new AssignmentsClient(conn);
        var assignment = new Assignment { Name = "An assignment" };

        // Act
        var ex = await Assert.ThrowsAsync<ClientException>(
            async () => await client.Update(
                987,
                149,
                assignment,
                cancellationToken: TestContext.Current.CancellationToken));

        // Assert
        ex.Message.ShouldBe("No assignment returned from Canvas");
    }

    [Fact]
    public async Task UpdateChecksName()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        var client = new AssignmentsClient(conn);
        var assignment = new Assignment { Name = string.Empty };

        // Act
        var ex = await Assert.ThrowsAsync<ClientException>(
            async () => await client.Update(
                987,
                149,
                assignment,
                cancellationToken: TestContext.Current.CancellationToken));

        // Assert
        ex.Message.ShouldBe("Assignments must have a name - cannot be null or empty");
    }
}