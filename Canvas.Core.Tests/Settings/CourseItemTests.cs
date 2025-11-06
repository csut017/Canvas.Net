using Canvas.Core.Entities;
using Canvas.Core.Settings;

namespace Canvas.Core.Tests.Settings;

[TestSubject(typeof(CourseItem))]
public class CourseItemTests
{
    [Theory]
    [InlineData(CourseInclude.None)]
    [InlineData(CourseInclude.Teachers)]
    [InlineData(CourseInclude.Teachers | CourseInclude.Term)]
    public void ImplicitFromIncludeWorks(CourseInclude include)
    {
        // Act
        CourseItem settings = include;

        // Assert
        settings.Options.ShouldBe(include);
    }

    [Fact]
    public void ToParametersHandlesMultipleEnrolmentTypes()
    {
        // Arrange
        var settings = new CourseItem(EnrolmentType.Teacher, EnrolmentType.Student);

        // Act
        var parms = settings.ToParameters();

        // Assert
        var actual = parms.ToString();
        actual.ShouldBe("?enrollment_type=teacher&enrollment_type=student");
    }

    [Theory]
    [InlineData(CourseInclude.None, "")]
    [InlineData(CourseInclude.Teachers, "?include[]=teachers")]
    [InlineData(CourseInclude.Teachers | CourseInclude.Term, "?include[]=term&include[]=teachers")]
    public void ToParametersHandlesOptions(CourseInclude include, string expected)
    {
        // Arrange
        var settings = new CourseItem
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