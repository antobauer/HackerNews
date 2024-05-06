using System.Net;
using System.Text.Json;
using HackerNews.Application.Models;
using HackerNews.Application.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HackerNews.Test.Services;
public class HackerNewsServiceTest
{
    private HackerNewsService _service;
    private IHttpClientFactory _httpClientFactory;
    private IMemoryCache _memoryCache = null!;
    private IConfiguration _configuration;
    private ILogger<HackerNewsService> _logger;

    [SetUp]
    public void SetUp()
    {
        _httpClientFactory = Substitute.For<IHttpClientFactory>();

        _memoryCache = Substitute.For<IMemoryCache>();
        _configuration = Substitute.For<IConfiguration>();
        _logger = Substitute.For<ILogger<HackerNewsService>>();
        _configuration["ConnectionSettings:BaseUrl"].Returns("https://hacker-news.firebaseio.com/v0/");
        _service = new HackerNewsService(_httpClientFactory, _memoryCache, _configuration, _logger);
    }

    [Test]
    public async Task GetNewStoriesAsync_NoCache_FetchesStories()
    {
        // Arrange
        var idResponse = "[1, 2, 3]";
        var storyDetailsResponses = new Dictionary<string, string>
        {
            { "http://localhost/item/1.json", JsonSerializer.Serialize(new Story { Id = 1, Title = "Story 1" }) },
            { "http://localhost/item/2.json", JsonSerializer.Serialize(new Story { Id = 2, Title = "Story 2" }) },
            { "http://localhost/item/3.json", JsonSerializer.Serialize(new Story { Id = 3, Title = "Story 3" }) }
        };

        var mockHttpMessageHandler = new MockHttpMessageHandler(storyDetailsResponses);
        var httpClient = new HttpClient(mockHttpMessageHandler)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        storyDetailsResponses.Add("http://localhost/newstories.json", idResponse);

        List<Story> nullStories = null;
        _memoryCache.TryGetValue("new-stories", out Arg.Any<List<Story>>()).Returns(x =>
        {
            x[1] = nullStories;
            return false;
        });

        // Act
        var result = await _service.GetNewStoriesAsync(1, 3, "");


        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.TotalCount, Is.EqualTo(3));
            Assert.That(result.Items.Count, Is.EqualTo(3));
        });
        Assert.That(result.Items, Is.EquivalentTo(new List<Story>
        {
            new() { Id = 1, Title = "Story 1" },
            new() { Id = 2, Title = "Story 2" },
            new() { Id = 3, Title = "Story 3" }
        }));
    }

    [Test]
    public async Task GetNewStoriesAsync_WithCache_UsesCachedData()
    {
        // Arrange
        var cachedStories = new List<Story>
        {
            new() { Id = 1, Title = "Cached Story 1" },
            new() { Id = 2, Title = "Cached Story 2" }
        };

        _memoryCache.TryGetValue("new-stories", out Arg.Any<List<Story>>()).Returns(x =>
        {
            x[1] = cachedStories;
            return true;
        });

        var currentStoryIds = "[1, 2, 3]";
        var storyDetailsResponses = new Dictionary<string, string>
        {
            { "http://localhost/item/3.json", JsonSerializer.Serialize(new Story { Id = 3, Title = "New Story 3" }) }
        };

        var mockHttpMessageHandler = new MockHttpMessageHandler(storyDetailsResponses);
        var httpClient = new HttpClient(mockHttpMessageHandler)
        {
            BaseAddress = new Uri("http://localhost/")
        };
        _httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        storyDetailsResponses.Add("http://localhost/newstories.json", currentStoryIds);

        // Act
        var result = await _service.GetNewStoriesAsync();

        // Assert
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result, Contains.Item(new Story { Id = 1, Title = "Cached Story 1" }));
        Assert.That(result, Contains.Item(new Story { Id = 2, Title = "Cached Story 2" }));
        Assert.That(result, Contains.Item(new Story { Id = 3, Title = "New Story 3" }));
    }


    internal class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Dictionary<string, string> _responsesByUri;
        private readonly HttpStatusCode _statusCode;

        public MockHttpMessageHandler(Dictionary<string, string> responsesByUri, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _responsesByUri = responsesByUri;
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_responsesByUri.TryGetValue(request.RequestUri.ToString(), out var response))
            {
                return Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = _statusCode,
                    Content = new StringContent(response)
                });
            }

            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });
        }
    }

    private void Dispose()
    {
        if (_memoryCache is IDisposable disposableCache)
        {
            disposableCache.Dispose();
        }
    }

    [TearDown]
    public void TearDown()
    {
        Dispose();
    }
}