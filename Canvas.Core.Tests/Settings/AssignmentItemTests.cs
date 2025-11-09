using Canvas.Core.Settings;

namespace Canvas.Core.Tests.Settings;

[TestSubject(typeof(AssignmentItem))]
public class AssignmentItemTests
{
    [Theory]
    [InlineData(AssignmentInclude.None)]
    [InlineData(AssignmentInclude.AllDates)]
    public void ImplicitFromIncludeWorks(AssignmentInclude include)
    {
        // Act
        AssignmentItem settings = include;

        // Assert
        settings.Options.ShouldBe(include);
    }

    [Theory]
    [InlineData(AssignmentInclude.None, "")]
    [InlineData(AssignmentInclude.AllDates, "?include[]=all_dates")]
    public void ToParametersHandlesOptions(AssignmentInclude include, string expected)
    {
        // Arrange
        var settings = new AssignmentItem
        {
            Options = include
        };

        // Act
        var parms = settings.ToParameters();

        // Assert
        var actual = parms.ToString();
        actual.ShouldBe(expected);
    }
}