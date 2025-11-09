using Canvas.Core.Settings;

namespace Canvas.Core.Tests.Settings;

[TestSubject(typeof(SubmissionList))]
public class SubmissionListTests
{
    [Theory]
    [InlineData(SubmissionInclude.None)]
    [InlineData(SubmissionInclude.User)]
    [InlineData(SubmissionInclude.SubmissionComments)]
    [InlineData(SubmissionInclude.RubricAssessment)]
    [InlineData(SubmissionInclude.All)]
    public void ImplicitFromIncludeWorks(SubmissionInclude include)
    {
        // Act
        SubmissionList settings = include;

        // Assert
        settings.Options.ShouldBe(include);
    }

    [Theory]
    [InlineData(SubmissionInclude.None, "?per_page=50")]
    [InlineData(SubmissionInclude.User, "?per_page=50&include[]=user")]
    public void ToParametersHandlesOptions(SubmissionInclude include, string expected)
    {
        // Arrange
        var settings = new SubmissionList
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