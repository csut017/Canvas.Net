using Canvas.Core.Settings;

namespace Canvas.Core.Tests.Settings;

[TestSubject(typeof(AssignmentList))]
public class AssignmentListTests
{
    [Theory]
    [InlineData(AssignmentInclude.None)]
    [InlineData(AssignmentInclude.AllDates)]
    public void ImplicitFromIncludeWorks(AssignmentInclude include)
    {
        // Act
        AssignmentList settings = include;

        // Assert
        settings.Options.ShouldBe(include);
    }

    [Theory]
    [InlineData(AssignmentInclude.None, "?per_page=50")]
    [InlineData(AssignmentInclude.AllDates, "?per_page=50&include[]=all_dates")]
    public void ToParametersHandlesOptions(AssignmentInclude include, string expected)
    {
        // Arrange
        var settings = new AssignmentList
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