using Canvas.Core.Entities;

namespace Canvas.Core.Tests.Entities;

[TestSubject(typeof(User))]
public class UserTests
{
    [Theory]
    [InlineData("Bob", "Bill", "Bill")]
    [InlineData("Bob", "", "Bob")]
    [InlineData("Bob", null, "Bob")]
    public void GetDisplayName(string name, string displayName, string expected)
    {
        // Arrange
        var user = new User { Name = name, NameDisplay = displayName };

        // Act
        var actual = user.GetDisplayName();

        // Assert
        actual.ShouldBe(expected);
    }
}