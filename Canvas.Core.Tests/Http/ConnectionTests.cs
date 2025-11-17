using Canvas.Core.Http;
using Canvas.Core.Settings;
using FakeItEasy;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Canvas.Core.Tests.Http;

[TestSubject(typeof(Connection))]
public class ConnectionTests
{
    [Fact]
    public void ConstructorAddsToken()
    {
        // Arrange
        var client = new HttpClient();

        // Act
        _ = new Connection("http://canvas.com", "1234", client: client);

        // Assert
        var token = client.DefaultRequestHeaders.Authorization?.Parameter;
        token.ShouldBe("1234");
        var scheme = client.DefaultRequestHeaders.Authorization?.Scheme;
        scheme.ShouldBe("Bearer");
    }

    [Theory]
    [InlineData("http://canvas.com")]
    [InlineData("http://canvas.com/")]
    [InlineData("http://canvas.com/api")]
    [InlineData("http://canvas.com/api/v1")]
    [InlineData("http://canvas.com/api/v1/")]
    public void ConstructorTrimsUrl(string inputUrl)
    {
        // Arrange
        var client = new HttpClient();

        // Act
        var conn = new Connection(inputUrl, "1234", client: client);

        // Assert
        conn.ShouldSatisfyAllConditions(
            () => conn.BaseAddress.ShouldNotBeNull(),
            () => conn.BaseAddress?.ToString().ShouldBe("http://canvas.com/")
        );
    }

    [Fact]
    public async Task GetCallsClient()
    {
        // Arrange
        var handler = new FakeJsonHandler(new { }, HttpStatusCode.OK);
        var client = new HttpClient(handler);
        var conn = new Connection("http://canvas.com", "1234", client: client);

        // Act
        var resp = await conn.Get(
            "api/v1/test",
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        resp.ShouldSatisfyAllConditions(
            () => resp.ShouldNotBeNull(),
            () => resp.StatusCode.ShouldBe(HttpStatusCode.OK)
        );
        var content = await resp.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        content.ShouldBe("{}");
        handler.Method.ShouldBe(HttpMethod.Get);
    }

    [Fact]
    public async Task GetThrowsException()
    {
        // Arrange
        var client = new HttpClient(
            new FakeJsonHandler(new { }, HttpStatusCode.BadRequest));
        var conn = new Connection("http://canvas.com", "1234", client: client);

        // Act
        var ex = await Assert.ThrowsAsync<ConnectionException>(
            async () => await conn.Get("api/v1/test", cancellationToken: TestContext.Current.CancellationToken));

        // Assert
        ex.ShouldSatisfyAllConditions(
            () => ex.Message.ShouldBe("Canvas returned a non-success response code [BadRequest]"),
            () => ex.Url.ShouldBe("api/v1/test"),
            () => ex.Content.ShouldBe("{}")
        );
    }

    [Fact]
    public async Task ListHandlesMultiplePages()
    {
        // Arrange
        var data = new List<FakeItem> {
            new FakeItem { Name = "1" }.WithHeader("Link", "<http://canvas.com/next>; rel=\"next\""),
            new () { Name = "2" },
        };
        var handler = new FakeHandler(data.Select(i => i.AsMessage(HttpStatusCode.OK)).ToArray());
        var client = new HttpClient(handler);
        var conn = new Connection("http://canvas.com", "1234", client: client);

        // Act
        var received = await conn
            .List<FakeItem>("api/v1/test", new List(), cancellationToken: TestContext.Current.CancellationToken)
            .ToListAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        received.Select(i => i.Name)
            .ToList()
            .ShouldBeEquivalentTo(data.Select(i => i.Name).ToList());
    }

    [Fact]
    public async Task ListRetrievesData()
    {
        // Arrange
        var data = new List<FakeItem> {
            new () { Name = "1"},
            new () { Name = "2" },
        };
        var handler = new FakeJsonHandler(data, HttpStatusCode.OK);
        var client = new HttpClient(handler);
        var conn = new Connection("http://canvas.com", "1234", client: client);

        // Act
        var received = await conn
            .List<FakeItem>("api/v1/test", new List(), cancellationToken: TestContext.Current.CancellationToken)
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        received.ToList().ShouldBeEquivalentTo(data);
    }

    [Fact]
    public async Task ListStopsAtMaxPages()
    {
        // Arrange
        var data = new List<FakeItem> {
            new FakeItem { Name = "1" }.WithHeader("Link", "<http://canvas.com/next>; rel=\"next\""),
            new FakeItem { Name = "2" }.WithHeader("Link", "<http://canvas.com/next>; rel=\"next\""),
            new () { Name = "3" },
        };
        var handler = new FakeHandler(data.Select(i => i.AsMessage(HttpStatusCode.OK)).ToArray());
        var client = new HttpClient(handler);
        var conn = new Connection("http://canvas.com", "1234", client: client);
        var settings = new List
        {
            MaxPages = 2,
        };

        // Act
        var received = await conn
            .List<FakeItem>("api/v1/test", settings, cancellationToken: TestContext.Current.CancellationToken)
            .ToListAsync(TestContext.Current.CancellationToken);

        // Assert
        received.Select(i => i.Name)
            .ToList()
            .ShouldBeEquivalentTo(data.Take(2).Select(i => i.Name).ToList());
    }

    [Fact]
    public async Task PostJsonCallsClientAndDeserializesJson()
    {
        // Arrange
        var data = new FakeDataItem(1124);
        var handler = new FakeJsonHandler(data, HttpStatusCode.OK);
        var client = new HttpClient(handler);
        var conn = new Connection("http://canvas.com", "1234", client: client);

        // Act
        var resp = await conn.PostJson<FakeDataItem>(
            "api/v1/test",
            data,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        resp.ShouldSatisfyAllConditions(
            () => resp.ShouldNotBeNull(),
            () => resp?.Id.ShouldBe(1124)
        );
        handler.Data.ShouldBe("{\"id\":1124}");
    }

    [Fact]
    public async Task PostJsonChecksResponseCode()
    {
        // Arrange
        var data = new FakeDataItem(1124);
        var handler = new FakeJsonHandler(data, HttpStatusCode.BadRequest);
        var client = new HttpClient(handler);
        var conn = new Connection("http://canvas.com", "1234", client: client);

        // Act
        var ex = await Assert.ThrowsAsync<ConnectionException>(async () => await conn.PostJson<FakeDataItem>(
            "api/v1/test",
            data,
            cancellationToken: TestContext.Current.CancellationToken));

        // Assert
        ex.Message.ShouldBe("Canvas returned a non-success response code [BadRequest]");
    }

    [Fact]
    public async Task PutFormCallsClientAndDeserializesJson()
    {
        // Arrange
        var data = new FakeDataItem(1124);
        var handler = new FakeJsonHandler(data, HttpStatusCode.OK);
        var client = new HttpClient(handler);
        var conn = new Connection("http://canvas.com", "1234", client: client);
        var values = Parameters.New()
            .Add("name", "value");

        // Act
        var resp = await conn.PutForm<FakeDataItem>(
            "api/v1/test",
            values,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        resp.ShouldBeEquivalentTo(data);
        handler.Data.ShouldBe("name=value");
    }

    [Fact]
    public async Task PutJsonCallsClientAndDeserializesJson()
    {
        // Arrange
        var data = new FakeDataItem(1124);
        var handler = new FakeJsonHandler(data, HttpStatusCode.OK);
        var client = new HttpClient(handler);
        var conn = new Connection("http://canvas.com", "1234", client: client);

        // Act
        var resp = await conn.PutJson<FakeDataItem>(
            "api/v1/test",
            data,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        resp.ShouldBeEquivalentTo(data);
        handler.Data.ShouldBe("{\"id\":1124}");
    }

    [Fact]
    public async Task PutJsonChecksResponseCode()
    {
        // Arrange
        var data = new FakeDataItem(1124);
        var handler = new FakeJsonHandler(data, HttpStatusCode.BadRequest);
        var client = new HttpClient(handler);
        var conn = new Connection("http://canvas.com", "1234", client: client);

        // Act
        var ex = await Assert.ThrowsAsync<ConnectionException>(async () => await conn.PutJson<FakeDataItem>(
            "api/v1/test",
            data,
            cancellationToken: TestContext.Current.CancellationToken));

        // Assert
        ex.Message.ShouldBe("Canvas returned a non-success response code [BadRequest]");
    }

    [Fact]
    public async Task RetrieveAppendsParameters()
    {
        // Arrange
        var handler = new FakeJsonHandler(new FakeItem { Name = "Testing" }, HttpStatusCode.OK);
        var client = new HttpClient(handler);
        var conn = new Connection("http://canvas.com", "1234", client: client);

        // Act
        _ = await conn.Retrieve<FakeItem>(
            "api/v1/test",
            new Parameters { { "Test", 123 }, },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        handler.Url.ShouldBe("http://canvas.com/api/v1/test?Test=123");
    }

    [Fact]
    public async Task RetrieveGetsAnItem()
    {
        // Arrange
        var handler = new FakeJsonHandler(new FakeItem { Name = "Testing" }, HttpStatusCode.OK);
        var client = new HttpClient(handler);
        var conn = new Connection("http://canvas.com", "1234", client: client);

        // Act
        var item = await conn.Retrieve<FakeItem>(
            "api/v1/test",
            [],
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        item.ShouldSatisfyAllConditions(
            () => item.ShouldNotBeNull(),
            () => item?.Name.ShouldBe("Testing")
        );
    }

    [Fact]
    public async Task RetrieveHandlesNonSuccessStatus()
    {
        // Arrange
        var handler = new FakeJsonHandler(new { }, HttpStatusCode.BadRequest);
        var client = new HttpClient(handler);
        var conn = new Connection("http://canvas.com", "1234", client: client);

        // Act
        var item = await conn.Retrieve<FakeItem>(
            "api/v1/test",
            [],
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        item.ShouldBeNull();
    }

    [Fact]
    public void UpdateLoggerSetsLogger()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var handler = new FakeJsonHandler(new { }, HttpStatusCode.BadRequest);
        var client = new HttpClient(handler);
        var conn = new Connection("http://canvas.com", "1234", client: client);

        // Act
        conn.UpdateLogger(logger);

        // Assert
        conn.Logger.ShouldNotBeNull();
    }

    public class FakeHandler(params HttpResponseMessage[] messages)
                                : HttpMessageHandler
    {
        private int _position;

        public int NumberOfCalls => _position;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(messages[_position++]);
        }
    }

    public class FakeItem
    {
        [JsonIgnore]
        public List<Tuple<string, string>> Headers { get; } = [];

        [JsonPropertyName("name")]
        public string Name { get; set; }

        public HttpResponseMessage AsMessage(HttpStatusCode statusCode)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(JsonSerializer.Serialize(new List<FakeItem> { this })),
            };
            foreach (var header in Headers)
            {
                response.Headers.Add(header.Item1, header.Item2);
            }
            return response;
        }

        public FakeItem WithHeader(string name, string value)
        {
            Headers.Add(Tuple.Create(name, value));
            return this;
        }
    }

    public class FakeJsonHandler(object data, HttpStatusCode statusCode)
                : HttpMessageHandler
    {
        public string Data { get; private set; }

        public HttpMethod Method { get; private set; }

        public string Url { get; private set; } = string.Empty;

        protected sealed override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Url = request.RequestUri?.ToString() ?? string.Empty;
            Method = request.Method;
            if (request.Content != null) Data = await request.Content!.ReadAsStringAsync(cancellationToken);
            var response = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(JsonSerializer.Serialize(data)),
            };
            return response;
        }
    }

    private record FakeDataItem([property: JsonPropertyName("id")] int Id);
}