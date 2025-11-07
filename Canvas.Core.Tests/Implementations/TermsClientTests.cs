using System.Threading;
using System.Threading.Tasks;
using Canvas.Core.Entities;
using Canvas.Core.Implementations;
using FakeItEasy;

namespace Canvas.Core.Tests.Implementations;

[TestSubject(typeof(TermsClient))]
public class TermsClientTests
{
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