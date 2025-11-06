using Canvas.Core.Entities;

namespace Canvas.Core.Tests.Entities;

[TestSubject(typeof(Course))]
public class CourseTests
{
    [Theory]
    [InlineData("available", true)]
    [InlineData("other", false)]
    public void IsAvailableChecksState(string state, bool expected)
    {
        // Arrange
        var course = new Course
        {
            State = state,
        };

        // Act
        var actual = course.IsAvailable;

        // Assert
        course.IsAvailable.ShouldBe(expected);
    }
}