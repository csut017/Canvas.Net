using Canvas.Core.Entities;

namespace Canvas.Core.Tests.Entities;

[TestSubject(typeof(EntityWithId.EquityComparer))]
public class EntityWithIdEquityComparerTests
{
    [Fact]
    public void EqualsHandlesDifferentTypes()
    {
        // Arrange
        var entity1 = new FakeEntity1 { Id = 5 };
        var entity2 = new FakeEntity2 { Id = 5 };
        var comparer = new EntityWithId.EquityComparer();

        // Act
        var actual = comparer.Equals(entity1, entity2);

        // Assert
        actual.ShouldBeFalse();
    }

    [Fact]
    public void EqualsHandlesFirstEntityIsNull()
    {
        // Arrange
        var entity = new FakeEntity1 { Id = 5 };
        var comparer = new EntityWithId.EquityComparer();

        // Act
        var actual = comparer.Equals(null, entity);

        // Assert
        actual.ShouldBeFalse();
    }

    [Fact]
    public void EqualsHandlesSameEntity()
    {
        // Arrange
        var entity = new FakeEntity1 { Id = 5 };
        var comparer = new EntityWithId.EquityComparer();

        // Act
        var actual = comparer.Equals(entity, entity);

        // Assert
        actual.ShouldBeTrue();
    }

    [Fact]
    public void EqualsHandlesSameTypeAndIds()
    {
        // Arrange
        var entity1 = new FakeEntity1 { Id = 5 };
        var entity2 = new FakeEntity1 { Id = 5 };
        var comparer = new EntityWithId.EquityComparer();

        // Act
        var actual = comparer.Equals(entity1, entity2);

        // Assert
        actual.ShouldBeTrue();
    }

    [Fact]
    public void EqualsHandlesSameTypeButDifferentIds()
    {
        // Arrange
        var entity1 = new FakeEntity1 { Id = 5 };
        var entity2 = new FakeEntity1 { Id = 7 };
        var comparer = new EntityWithId.EquityComparer();

        // Act
        var actual = comparer.Equals(entity1, entity2);

        // Assert
        actual.ShouldBeFalse();
    }

    [Fact]
    public void EqualsHandlesSecondEntityIsNull()
    {
        // Arrange
        var entity = new FakeEntity1 { Id = 5 };
        var comparer = new EntityWithId.EquityComparer();

        // Act
        var actual = comparer.Equals(entity, null);

        // Assert
        actual.ShouldBeFalse();
    }

    [Fact]
    public void GetHashCodeReturnsHashOfId()
    {
        // Arrange
        var entity = new FakeEntity1 { Id = 5 };
        var comparer = new EntityWithId.EquityComparer();

        // Act
        var hash = comparer.GetHashCode(entity);

        // Assert
        hash.ShouldBe(5.GetHashCode());
    }

    public record FakeEntity1 : EntityWithId;
    public record FakeEntity2 : EntityWithId;
}