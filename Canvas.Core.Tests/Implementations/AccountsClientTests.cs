using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Canvas.Core.Clients;
using Canvas.Core.Entities;
using Canvas.Core.Implementations;
using Canvas.Core.Settings;
using FakeItEasy;

namespace Canvas.Core.Tests.Implementations;

[TestSubject(typeof(AccountsClient))]
public class AccountsClientTests
{
    [Fact]
    public async Task ListForAccountViaEntityCallsUnderlyingConnection()
    {
        // Arrange
        var data = new List<Account>
        {
            new() { Id = 1, },
            new() { Id = 2, },
        };
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.List<Account>(
                "/api/v1/accounts",
                A<List>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(data.ToAsyncEnumerable());
        var client = new AccountsClient(conn);

        // Act
        var courses = await client.ListForCurrentUser(
                cancellationToken: TestContext.Current.CancellationToken)
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        courses.ShouldBe(data);
    }

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

    [Fact]
    public void TermsReturnsAClient()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        var client = new AccountsClient(conn);

        // Act
        var child = client.Terms;

        // Assert
        child.ShouldSatisfyAllConditions(
            () => child.ShouldNotBeNull(),
            () => child.ShouldBeAssignableTo<ITerms>()
        );
    }
}