using Canvas.Core.Entities;
using Canvas.Core.Settings;

namespace Canvas.Core.Tests.Settings;

[TestSubject(typeof(CourseList))]
public class CourseListTests
{
    [Theory]
    [InlineData(CourseInclude.None)]
    [InlineData(CourseInclude.Teachers)]
    [InlineData(CourseInclude.Teachers | CourseInclude.Term)]
    public void ImplicitFromIncludeWorks(CourseInclude include)
    {
        // Act
        CourseList settings = include;

        // Assert
        settings.Options.ShouldBe(include);
    }

    [Theory]
    [InlineData(CourseInclude.None, "?per_page=50")]
    [InlineData(CourseInclude.Teachers, "?per_page=50&include[]=teachers")]
    [InlineData(CourseInclude.Teachers | CourseInclude.Term, "?per_page=50&include[]=term&include[]=teachers")]
    public void ToParametersHandlesOptions(CourseInclude include, string expected)
    {
        // Arrange
        var settings = new CourseList
        {
            Options = include
        };

        // Act
        var parms = settings.ToParameters();

        // Assert
        var actual = parms.ToString();
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(null, null, null, "?per_page=50")]
    [InlineData(EnrolmentType.Student, null, null, "?per_page=50&enrollment_type=student")]
    [InlineData(EnrolmentType.Student, CourseInclude.Term, null, "?per_page=50&enrollment_type=student&include[]=term")]
    [InlineData(EnrolmentType.Student, null, 1UL, "?per_page=50&enrollment_type=student&enrollment_term_id=1")]
    [InlineData(null, CourseInclude.Term, null, "?per_page=50&include[]=term")]
    [InlineData(null, CourseInclude.Term, 1UL, "?per_page=50&include[]=term&enrollment_term_id=1")]
    [InlineData(null, null, 1UL, "?per_page=50&enrollment_term_id=1")]
    [InlineData(EnrolmentType.Student, CourseInclude.Term, 1UL, "?per_page=50&enrollment_type=student&include[]=term&enrollment_term_id=1")]
    public void ToParametersSetsValues(EnrolmentType? enrolmentType, CourseInclude? include, ulong? term, string expected)
    {
        // Arrange
        var settings = new CourseList
        {
            EnrolmentType = enrolmentType,
            Options = include.GetValueOrDefault(CourseInclude.None),
            TermId = term,
        };

        // Act
        var parms = settings.ToParameters();

        // Assert
        var actual = parms.ToString();
        actual.ShouldBe(expected);
    }
}