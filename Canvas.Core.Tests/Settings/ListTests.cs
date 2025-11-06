using Canvas.Core.Settings;

namespace Canvas.Core.Tests.Settings;

[TestSubject(typeof(List))]
public class ListTests
{
    [Fact]
    public void ToParametersGeneratesParameters()
    {
        // Arrange
        var settings = new List();

        // Act
        var parms = settings.ToParameters();

        // Assert
        var actual = parms.ToString();
        actual.ShouldBe("?per_page=50");
    }

    [Theory]
    [InlineData(null, null, "?per_page=50")]
    [InlineData(null, 1, "?per_page=50&page=1")]
    [InlineData(10, null, "?per_page=10")]
    [InlineData(10, 1, "?per_page=10&page=1")]
    [InlineData(1_000, null, "?per_page=100")]
    public void ToParametersSetsValues(int? pageSize, int? pageStart, string expected)
    {
        // Arrange
        var settings = new List
        {
            PageSize = pageSize,
            PageStart = pageStart,
        };

        // Act
        var parms = settings.ToParameters();

        // Assert
        var actual = parms.ToString();
        actual.ShouldBe(expected);
    }
}