using Canvas.Core.Http;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Canvas.Core.Tests.Http;

[TestSubject(typeof(ConfigurationExtensions))]
public class ConfigurationExtensionsTests
{
    [Fact]
    public async Task LogIncomingContentHandlesJson()
    {
        // Arrange
        var message = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Data", MediaTypeHeaderValue.Parse("application/json")),
        };
        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.InMemory()
            .CreateLogger();
        var conn = new Connection("test", "test");
        conn.UpdateLogger(logger);

        // Act
        var stream = await ConfigurationExtensions.LogIncomingContent(
            conn,
            message,
            TestContext.Current.CancellationToken);

        // Assert
        InMemorySink.Instance
            .Should()
            .HaveMessage("Received: {content}")
            .Appearing()
            .Once()
            .WithLevel(LogEventLevel.Debug);
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
        content.ShouldBe("Data");
    }

    [Fact]
    public void ViaHttpSetsConnection()
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

    [Fact]
    public void ViaHttpWithResponseLoggingSetsConnection()
    {
        // Arrange
        var config = new ClientConfiguration();

        // Act
        var result = config.ViaHttpWithResponseLogging("http://test", "1234");

        // Assert
        result.ShouldBeSameAs(config);
        var http = config.Connection.ShouldBeOfType<Connection>();
        http.BaseAddress.ShouldBe("http://test/");
    }
}