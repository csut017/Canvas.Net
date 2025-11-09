using Canvas.Core.Entities;

namespace Canvas.Core.Tests.Entities;

[TestSubject(typeof(SubmissionSummary))]
public class SubmissionSummaryTests
{
    [Fact]
    public void TotalAddsAllComponents()
    {
        // Arrange
        var item = new SubmissionSummary
        {
            Graded = 1,
            NotSubmitted = 2,
            Ungraded = 3,
        };

        // Act
        var actual = item.Total;

        // Assert
        actual.ShouldBe(6);
    }
}