using Canvas.Core.Settings;
using FakeItEasy;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Canvas.Core.Tests;

[TestSubject(typeof(ClientConfiguration))]
public class ClientConfigurationTests
{
    [Fact]
    public void BuildChecksForConnection()
    {
        // Arrange
        var config = new ClientConfiguration();

        // Act
        var ex = Assert.Throws<ConfigurationException>(() => config.Build());

        // Assert
        ex.Message.ShouldBe("Connection must be initialised.");
    }

    [Fact]
    public void BuildGeneratesClient()
    {
        // Arrange
        var config = new ClientConfiguration
        {
            Connection = new FakeConnection()
        };

        // Act
        var client = config.Build();

        // Assert
        client.ShouldBeOfType<Client>();
    }

    [Fact]
    public void BuildSetsLoggerOnConnection()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var conn = new FakeConnection();
        var config = new ClientConfiguration
        {
            Connection = conn
        }.UseLogger(logger);

        // Act
        var client = config.Build();

        // Assert
        conn.Logger.ShouldNotBeNull();
    }

    [Fact]
    public void UseLoggerSetsTheLogger()
    {
        // Arrange
        var config = new ClientConfiguration();
        var logger = A.Fake<ILogger>();

        // Act
        var result = config.UseLogger(logger);

        // Assert
        result.ShouldBeSameAs(config);
        result.Logger.ShouldBeSameAs(logger);
    }

    public class FakeConnection
        : ILoggingConnection
    {
        public ILogger Logger { get; private set; }

        public Task<HttpResponseMessage> Get(string url, bool throwExceptionOnFailure = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<TItem> List<TItem>(string url, List settings = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TItem> PostJson<TItem>(string url, object item, bool throwExceptionOnFailure = true,
            CancellationToken cancellationToken = default) where TItem : class
        {
            throw new NotImplementedException();
        }

        public Task<TItem> PutJson<TItem>(string url, object item, bool throwExceptionOnFailure = true,
            CancellationToken cancellationToken = default) where TItem : class
        {
            throw new NotImplementedException();
        }

        public Task<TItem> Retrieve<TItem>(string url, Parameters parameters, CancellationToken cancellationToken = default) where TItem : class
        {
            throw new NotImplementedException();
        }

        public void UpdateLogger(ILogger logger)
        {
            Logger = logger;
        }
    }
}