using System;

namespace Canvas.Core.Tests;

[TestSubject(typeof(ConfigurationException))]
public class ConfigurationExceptionTests
{
    [Fact]
    public void ConstructorSetsMessage()
    {
        // Act
        var ex = new ConfigurationException("Testing");

        // Assert
        ex.Message.ShouldBe("Testing");
        ex.InnerException.ShouldBeNull();
    }

    [Fact]
    public void ConstructorSetsMessageAndInner()
    {
        // Arrange
        var inner = new Exception("Inner");

        // Act
        var ex = new ConfigurationException("Testing", inner);

        // Assert
        ex.Message.ShouldBe("Testing");
        ex.InnerException.ShouldBe(inner);
    }
}