using System;
using Canvas.Core.Entities;

namespace Canvas.Core.Tests;

[TestSubject(typeof(CanvasException))]
public class CanvasExceptionTests
{
    [Fact]
    public void ConstructorSetsProperties()
    {
        // Arrange
        const string urlValue = "http://abc";
        const string messageValue = "A message";
        var errors = new[] { new Error { Message = "Testing" } };

        // Act
        var ex = new CanvasException(urlValue, messageValue, errors);

        // Assert
        ex.ShouldSatisfyAllConditions(
            () => ex.Url.ShouldBe(urlValue),
            () => ex.Message.ShouldBe(messageValue),
            () => ex.InnerException.ShouldBeNull(),
            () => ex.Content.ShouldBeNull(),
            () => ex.Errors.ShouldBe(errors)
        );
    }

    [Fact]
    public void ConstructorWithInnerExceptionSetsProperties()
    {
        // Arrange
        const string urlValue = "http://abc";
        const string messageValue = "A message";
        var errors = new[] { new Error { Message = "Testing" } };
        var inner = new Exception();

        // Act
        var ex = new CanvasException(urlValue, messageValue, errors, inner);

        // Assert
        ex.ShouldSatisfyAllConditions(
            () => ex.Url.ShouldBe(urlValue),
            () => ex.Message.ShouldBe(messageValue),
            () => ex.InnerException.ShouldBeSameAs(inner),
            () => ex.Content.ShouldBeNull(),
            () => ex.Errors.ShouldBe(errors)
        );
    }
}