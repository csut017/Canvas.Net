using System.Threading;
using System.Threading.Tasks;
using Canvas.Core.Entities;
using Canvas.Core.Implementations;
using FakeItEasy;

namespace Canvas.Core.Tests.Implementations;

[TestSubject(typeof(CurrentUserClient))]
public class CurrentUserClientTests
{
    [Fact]
    public async Task GetCallsUnderlyingConnection()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.Retrieve<User>(
            "/api/v1/users/self",
            A<Parameters>.Ignored,
            A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(new User { Id = 1234 }));
        var client = new CurrentUserClient(conn);

        // Act
        var user = await client.Get(
            TestContext.Current.CancellationToken);

        // Assert
        user.ShouldSatisfyAllConditions(
            () => user.ShouldNotBeNull(),
            () => user?.Id.ShouldBe(1234UL)
        );
    }
}