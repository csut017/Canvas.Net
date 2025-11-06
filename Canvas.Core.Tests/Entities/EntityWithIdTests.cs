using Canvas.Core.Entities;

namespace Canvas.Core.Tests.Entities;

[TestSubject(typeof(EntityWithId))]
public class EntityWithIdTests
{
    [Fact]
    public void EqualsHandlesEntityWithDifferentId()
    {
        // Arrange
        var entity = new FakeEntity { Id = 123 };

        // Act
        var actual = entity.Equals(new FakeEntity { Id = 456 });

        // Assert
        actual.ShouldBeFalse();
    }

    [Fact]
    public void EqualsHandlesEntityWithSameId()
    {
        // Arrange
        var entity = new FakeEntity { Id = 123 };

        // Act
        var actual = entity.Equals(new FakeEntity { Id = 123 });

        // Assert
        actual.ShouldBeTrue();
    }

    [Fact]
    public void EqualsHandlesNonEntityType()
    {
        // Arrange
        var entity = new FakeEntity { Id = 123 };

        // Act
        // ReSharper disable once SuspiciousTypeConversion.Global
        var actual = entity.Equals("123");

        // Assert
        actual.ShouldBeFalse();
    }

    [Fact]
    public void GetHashCodeReturnsHashCodeOfId()
    {
        // Arrange
        var entity = new FakeEntity { Id = 123 };

        // Act
        var hashCode = entity.GetHashCode();

        // Assert
        hashCode.ShouldBe(123.GetHashCode());
    }

    public record FakeEntity : EntityWithId
    { }
}