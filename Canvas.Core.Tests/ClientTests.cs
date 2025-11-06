using Canvas.Core.Clients;
using FakeItEasy;

namespace Canvas.Core.Tests;

[TestSubject(typeof(Client))]
public class ClientTests
{
    [Fact]
    public void CurrentUserReturnsAClient()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        var client = new Client(conn);

        // Act
        var child = client.CurrentUser;

        // Assert
        child.ShouldSatisfyAllConditions(
            () => child.ShouldNotBeNull(),
            () => child.ShouldBeAssignableTo<ICurrentUser>()
        );
    }
}