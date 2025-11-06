using Canvas.Core.Entities;
using Canvas.Core.Implementations;
using FakeItEasy;
using System.Threading;
using System.Threading.Tasks;

namespace Canvas.Core.Tests.Implementations;

[TestSubject(typeof(CoursesClient))]
public class CoursesClientTests
{
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