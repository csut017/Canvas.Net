using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Canvas.Core.Entities;
using Canvas.Core.Implementations;
using Canvas.Core.Settings;
using FakeItEasy;

namespace Canvas.Core.Tests.Implementations;

[TestSubject(typeof(TermsClient))]
public class TermsClientTests
{
    [Fact]
    public async Task ListForAccountViaEntityCallsUnderlyingConnection()
    {
        // Arrange
        var data = new List<Term>
        {
            new() { Id = 1, },
            new() { Id = 2, },
        };
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.List<Term>(
                "/api/v1/accounts/7/terms",
                A<List>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(data.ToAsyncEnumerable());
        var client = new TermsClient(conn);

        // Act
        var courses = await client.ListForAccount(
                new Account { Id = 7 },
                cancellationToken: TestContext.Current.CancellationToken)
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        courses.ShouldBe(data);
    }

    [Fact]
    public async Task ListForAccountViaIdCallsUnderlyingConnection()
    {
        // Arrange
        var data = new List<Term>
        {
            new() { Id = 1, },
            new() { Id = 2, },
        };
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.List<Term>(
                "/api/v1/accounts/6/terms",
                A<List>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(data.ToAsyncEnumerable());
        var client = new TermsClient(conn);

        // Act
        var courses = await client.ListForAccount(
                6,
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
        A.CallTo(() => conn.Retrieve<Term>(
                "/api/v1/accounts/456/terms/8765",
                A<Parameters>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(new Term { Id = 1234 }));
        var client = new TermsClient(conn);

        // Act
        var term = await client.Retrieve(
            new Account { Id = 456 },
            new Term { Id = 8765 },
            TestContext.Current.CancellationToken);

        // Assert
        term.ShouldSatisfyAllConditions(
            () => term.ShouldNotBeNull(),
            () => term?.Id.ShouldBe(1234UL)
        );
    }

    [Fact]
    public async Task RetrieveByIdCallsUnderlyingConnection()
    {
        // Arrange
        var conn = A.Fake<IConnection>();
        A.CallTo(() => conn.Retrieve<Term>(
                "/api/v1/accounts/321/terms/1234",
                A<Parameters>.Ignored,
                A<CancellationToken>.Ignored))
            .Returns(Task.FromResult(new Term { Id = 1234 }));
        var client = new TermsClient(conn);

        // Act
        var term = await client.Retrieve(
            321,
            1234,
            TestContext.Current.CancellationToken);

        // Assert
        term.ShouldSatisfyAllConditions(
            () => term.ShouldNotBeNull(),
            () => term?.Id.ShouldBe(1234UL)
        );
    }
}