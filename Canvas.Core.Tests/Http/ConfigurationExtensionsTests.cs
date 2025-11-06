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
        var connConfig = new FakeConfig(config);

        // Act
        var result = connConfig.ViaHttp("http://test", "1234");

        // Assert
        result.ShouldBeSameAs(config);
        var http = connConfig.Connection.ShouldBeOfType<Connection>();
        http.BaseAddress.ShouldBe("http://test/");
    }

    public class FakeConfig(ClientConfiguration config)
        : IClientConnectionConfiguration
    {
        public IConnection Connection { get; private set; }
        public ClientConfiguration Owner => config;

        public void Connect(IConnection connection)
        {
            Connection = connection;
        }
    }
}