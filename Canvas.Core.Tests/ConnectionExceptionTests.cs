using System;

namespace Canvas.Core.Tests;

[TestSubject(typeof(ConnectionException))]
public class ConnectionExceptionTests
{
    [Fact]
    public void ConstructorSetsProperties()
    {
        // Arrange
        const string urlValue = "http://abc";
        const string messageValue = "A message";

        // Act
        var ex = new ConnectionException(urlValue, messageValue);

        // Assert
        ex.Url.ShouldBe(urlValue);
        ex.Message.ShouldBe(messageValue);
        ex.InnerException.ShouldBeNull();
        ex.Content.ShouldBeNull();
    }

    [Fact]
    public void ConstructorWithInnerExceptionSetsProperties()
    {
        // Arrange
        const string urlValue = "http://abc";
        const string messageValue = "A message";
        var inner = new Exception();

        // Act
        var ex = new ConnectionException(urlValue, messageValue, inner);

        // Assert
        ex.Url.ShouldBe(urlValue);
        ex.Message.ShouldBe(messageValue);
        ex.InnerException.ShouldBeSameAs(inner);
        ex.Content.ShouldBeNull();
    }
}