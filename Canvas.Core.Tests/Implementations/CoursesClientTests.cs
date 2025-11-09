using Canvas.Core.Clients;
using Canvas.Core.Entities;
using Canvas.Core.Implementations;
using Canvas.Core.Settings;
using FakeItEasy;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Canvas.Core.Tests.Implementations;

[TestSubject(typeof(CoursesClient))]
public class CoursesClientTests
{
    [Fact]
    public void AssignmentsReturnsAClient()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        var client = new CoursesClient(conn);

        // Act
        var child = client.Assignments;

        // Assert
        child.ShouldSatisfyAllConditions(
            () => child.ShouldNotBeNull(),
            () => child.ShouldBeAssignableTo<IAssignments>()
        );
    }

    [Fact]
    public async Task ListForAccountViaEntityCallsUnderlyingConnection()
    {
        // Arrange
        var data = new List<Course>
        {
            new() { Id = 1, },
            new() { Id = 2, },
        };
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.List<Course>(
                "/api/v1/accounts/32/courses",
                A<List>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(data.ToAsyncEnumerable());
        var client = new CoursesClient(conn);

        // Act
        var courses = await client.ListForAccount(
                new Account { Id = 32 },
                cancellationToken: TestContext.Current.CancellationToken)
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        courses.ShouldBe(data);
    }

    [Fact]
    public async Task ListForAccountViaIdCallsUnderlyingConnection()
    {
        // Arrange
        var data = new List<Course>
        {
            new() { Id = 1, },
            new() { Id = 2, },
        };
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.List<Course>(
                "/api/v1/accounts/6/courses",
                A<List>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(data.ToAsyncEnumerable());
        var client = new CoursesClient(conn);

        // Act
        var courses = await client.ListForAccount(
                6,
                cancellationToken: TestContext.Current.CancellationToken)
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        courses.ShouldBe(data);
    }

    [Fact]
    public async Task ListForCurrentUserCallsUnderlyingConnection()
    {
        // Arrange
        var data = new List<Course>
        {
            new() { Id = 1, },
            new() { Id = 2, },
        };
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.List<Course>(
                "/api/v1/courses",
                A<List>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(data.ToAsyncEnumerable());
        var client = new CoursesClient(conn);

        // Act
        var courses = await client.ListForCurrentUser(
                cancellationToken: TestContext.Current.CancellationToken)
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        courses.ShouldBe(data);
    }

    [Fact]
    public async Task RetrieveViaEntityCallsUnderlyingConnection()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.Retrieve<Course>(
                "/api/v1/courses/321",
                A<Parameters>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(new Course { Id = 321 }));
        var client = new CoursesClient(conn);

        // Act
        var course = await client.Retrieve(
            new Course { Id = 321 },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        course.ShouldSatisfyAllConditions(
            () => course.ShouldNotBeNull(),
            () => course?.Id.ShouldBe(321UL)
        );
    }

    [Fact]
    public async Task RetrieveViaIdCallsUnderlyingConnection()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.Retrieve<Course>(
                "/api/v1/courses/1234",
                A<Parameters>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(new Course { Id = 1234 }));
        var client = new CoursesClient(conn);

        // Act
        var course = await client.Retrieve(
            1234,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        course.ShouldSatisfyAllConditions(
            () => course.ShouldNotBeNull(),
            () => course?.Id.ShouldBe(1234UL)
        );
    }
}