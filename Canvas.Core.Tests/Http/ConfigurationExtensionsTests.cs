using Canvas.Core.Http;

namespace Canvas.Core.Tests.Http;

[TestSubject(typeof(ConfigurationExtensions))]
public class ConfigurationExtensionsTests
{
    [Fact]
    public void ExtensionMethodSetsConnection()
    {
        // Arrange
        var config = new ClientConfiguration();

        // Act
        var result = config.ViaHttp("http://test", "1234");

        // Assert
        result.ShouldBeSameAs(config);
        var http = config.Connection.ShouldBeOfType<Connection>();
        http.BaseAddress.ShouldBe("http://test/");
    }
}