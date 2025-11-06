using System;
using System.Net;
using System.Net.Http;

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

    [Fact]
    public void NewHandlesNoResponse()
    {
        // Arrange
        const string urlValue = "http://abc";

        // Act
        var ex = ConnectionException.New(urlValue, null);

        // Assert
        ex.Url.ShouldBe(urlValue);
        ex.Message.ShouldBe("Something went wrong while calling Canvas");
        ex.InnerException.ShouldBeNull();
        ex.Content.ShouldBeNull();
    }

    [Fact]
    public void NewHandlesResponse()
    {
        // Arrange
        const string urlValue = "http://abc";
        var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
        response.Content = new StringContent("Some content");

        // Act
        var ex = ConnectionException.New(urlValue, response);

        // Assert
        ex.Url.ShouldBe(urlValue);
        ex.Message.ShouldBe("Canvas returned a non-success response code [Forbidden]");
        ex.InnerException.ShouldBeNull();
        ex.Content.ShouldBe("Some content");
    }
}