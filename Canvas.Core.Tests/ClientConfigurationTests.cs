using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Canvas.Core.Settings;
using FakeItEasy;
using Serilog;

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
        : IConnection
    {
        public Task<HttpResponseMessage> Get(string url, bool throwExceptionOnFailure = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<TItem> List<TItem>(string url, Parameters parameters, List settings = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> Post(string url, IDictionary<string, string> formValues, bool throwExceptionOnFailure = true,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> Post(string url, Stream stream, string streamName, string fileName, IDictionary<string, string> formValues = null,
            bool throwExceptionOnFailure = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TItem> Post<TItem>(string url, IDictionary<string, string> formValues, bool throwExceptionOnFailure = true,
            CancellationToken cancellationToken = default) where TItem : class
        {
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> Put(string url, IDictionary<string, string> formValues, bool throwExceptionOnFailure = true,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TItem> Put<TItem>(string url, IDictionary<string, string> formValues, CancellationToken cancellationToken = default) where TItem : class
        {
            throw new NotImplementedException();
        }

        public Task<TItem> Retrieve<TItem>(string url, Parameters parameters, CancellationToken cancellationToken = default) where TItem : class
        {
            throw new NotImplementedException();
        }

        public Task<TItem> UploadFile<TItem>(Stream stream, string url, Dictionary<string, string> formValues, string fileName,
            CancellationToken cancellationToken = default) where TItem : class
        {
            throw new NotImplementedException();
        }
    }
}