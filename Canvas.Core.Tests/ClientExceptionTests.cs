using System;

namespace Canvas.Core.Tests;

[TestSubject(typeof(ClientException))]
public class ClientExceptionTests
{
    [Fact]
    public void ConstructorSetsMessage()
    {
        // Act
        var ex = new ClientException("Testing");

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
        var ex = new ClientException("Testing", inner);

        // Assert
        ex.Message.ShouldBe("Testing");
        ex.InnerException.ShouldBe(inner);
    }
}