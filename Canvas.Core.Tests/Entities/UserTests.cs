using System.Text.Json;
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

    [Fact]
    public void ParsesCurrentUserResponse()
    {
        // Arrange
        const string json = """{"id":1234,"name":"Bob Smith","created_at":"2025-01-02T16:00:00+12:00","sortable_name":"Smith, Bob","short_name":"Bob Smith","avatar_url":"https://canvas.com/images/thumbnails/123/456","last_name":"Smith","first_name":"Bob","locale":"en-AU","effective_locale":"en-AU","permissions":{"can_update_name":false,"can_update_avatar":true,"limit_parent_app_web_access":false}}""";

        // Act
        var item = JsonSerializer.Deserialize<User>(json, Defaults.SerializerOptions);

        // Assert
        item.ShouldSatisfyAllConditions(
            () => item.Id.ShouldBe(1234UL),
            () => item.Name.ShouldBe("Bob Smith"),
            () => item.NameSortable.ShouldBe("Smith, Bob"),
            () => item.NameFamily.ShouldBe("Smith"),
            () => item.NamePersonal.ShouldBe("Bob"),
            () => item.AvatarUrl.ShouldBe("https://canvas.com/images/thumbnails/123/456"),
            () => item.Locale.ShouldBe("en-AU")
        );
    }
}