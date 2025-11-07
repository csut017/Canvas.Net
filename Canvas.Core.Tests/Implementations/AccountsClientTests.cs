using System.Threading;
using System.Threading.Tasks;
using Canvas.Core.Entities;
using Canvas.Core.Implementations;
using FakeItEasy;

namespace Canvas.Core.Tests.Implementations;

[TestSubject(typeof(AccountsClient))]
public class AccountsClientTests
{
    [Fact]
    public async Task RetrieveByEntityCallsUnderlyingConnection()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.Retrieve<Account>(
                "/api/v1/accounts/1123",
                A<Parameters>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(new Account { Id = 1123 }));
        var client = new AccountsClient(conn);

        // Act
        var account = await client.Retrieve(
            new Account { Id = 1123 },
            TestContext.Current.CancellationToken);

        // Assert
        account.ShouldSatisfyAllConditions(
            () => account.ShouldNotBeNull(),
            () => account?.Id.ShouldBe(1123UL)
        );
    }

    [Fact]
    public async Task RetrieveByIdCallsUnderlyingConnection()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.Retrieve<Account>(
                "/api/v1/accounts/149",
                A<Parameters>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(new Account { Id = 149 }));
        var client = new AccountsClient(conn);

        // Act
        var account = await client.Retrieve(
            149,
            TestContext.Current.CancellationToken);

        // Assert
        account.ShouldSatisfyAllConditions(
            () => account.ShouldNotBeNull(),
            () => account?.Id.ShouldBe(149UL)
        );
    }
}