using Canvas.Core.Entities;

namespace Canvas.Core.Tests.Entities;

[TestSubject(typeof(EntityWithIdAndName))]
public class EntityWithIdAndNameTests
{
    [Theory]
    [InlineData("start", 6)]
    [InlineData("middle", 0)]
    [InlineData("end", -8)]
    public void CompareToHandlesEntity(string name, int expected)
    {
        // Arrange
        var entity = new FakeModel { Name = name };

        // Act
        var actual = entity.CompareTo(new FakeModel { Name = "middle" });

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void CompareToHandlesNonEntity()
    {
        // Arrange
        var entity = new FakeModel { Name = "Middle" };

        // Act
        var actual = entity.CompareTo("123");

        // Assert
        actual.ShouldBe(1);
    }

    public record FakeModel : EntityWithIdAndName
    { }
}