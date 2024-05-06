using System.Text.Json;
using HackerNews.Application.Interfaces;
using HackerNews.Application.Models;
using HackerNews.Application.Models.Paging;
using Microsoft.Extensions.Caching.Memory;

namespace HackerNews.Application.Services;

public class HackerNewsService : IHackerNewsService
{
    private IHttpClientFactory _httpClientFactory;
    private IMemoryCache _memoryCache;
    private IConfiguration _configuration;
    private ILogger<HackerNewsService> _logger;

    private static string _baseUrl = null;

    public HackerNewsService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IConfiguration configuration, ILogger<HackerNewsService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _memoryCache = memoryCache;
        _configuration = configuration;
        _baseUrl = _configuration.GetValue<string>("ConnectionSettings:BaseUrl")!;
        _logger = logger;
    }

    private static readonly JsonSerializerOptions? jsonSerializerOptions;

    static HackerNewsService()
    {
        jsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };
    }

    public async Task<PaginatedList<Story>> GetNewStoriesAsync(int page, int pageSize, string searchFilter)
    {
        const string cacheKey = "new-stories";
        var client = _httpClientFactory.CreateClient();
        var jsonResponse = await client.GetStringAsync($"{_baseUrl}newstories.json");
        var currentStoryIds = JsonSerializer.Deserialize<List<int>>(jsonResponse);

        List<Story> allStories;
        var isCacheUpdated = false;

        if (_memoryCache.TryGetValue(cacheKey, out List<Story> cachedStories))
        {
            var cachedStoryIds = cachedStories.Select(s => s.Id).ToHashSet();
            var newStoryIds = currentStoryIds.Except(cachedStoryIds).ToList();
            var obsoleteStoryIds = cachedStoryIds.Except(currentStoryIds).ToHashSet();

            // Fetch new stories
            var newStoriesTasks = newStoryIds.Select(GetStoryDetailsAsync);
            var newStories = await Task.WhenAll(newStoriesTasks);

            if (newStories.Any())
            {
                // Update cache by adding new stories and removing obsolete ones
                cachedStories.AddRange(newStories.Where(story => story != null));
                cachedStories.RemoveAll(story => obsoleteStoryIds.Contains(story.Id));

                isCacheUpdated = true;
            }

            allStories = cachedStories;
        }
        else
        {
            // First time fetch
            var storyDetailsTasks = currentStoryIds.Select(GetStoryDetailsAsync);
            var stories = await Task.WhenAll(storyDetailsTasks);
            allStories = stories.Where(story => story != null).ToList();
            isCacheUpdated = true;
        }

        if (isCacheUpdated)
        {
            _memoryCache.Set(cacheKey, allStories);
        }

        IEnumerable<Story> filteredStories = allStories;
        if (!string.IsNullOrWhiteSpace(searchFilter))
        {
            filteredStories = filteredStories.Where(s => s.Title != null && s.Title.Contains(searchFilter, StringComparison.OrdinalIgnoreCase));
        }

        var paginatedList = filteredStories
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedList<Story>(paginatedList, filteredStories.Count(), page, pageSize);
    }

    public async Task<List<Story>> GetNewStoriesAsync()
    {
        const string cacheKey = "new-stories";
        var client = _httpClientFactory.CreateClient();
        var jsonResponse = await client.GetStringAsync($"{_baseUrl}newstories.json");
        var currentStoryIds = JsonSerializer.Deserialize<List<int>>(jsonResponse);

        List<Story> allStories;
        var isCacheUpdated = false;

        if (_memoryCache.TryGetValue(cacheKey, out List<Story> cachedStories))
        {
            var cachedStoryIds = cachedStories.Select(s => s.Id).ToHashSet();
            var newStoryIds = currentStoryIds.Except(cachedStoryIds).ToList();
            var obsoleteStoryIds = cachedStoryIds.Except(currentStoryIds).ToHashSet();

            // Fetch new stories
            var newStoriesTasks = newStoryIds.Select(GetStoryDetailsAsync);
            var newStories = await Task.WhenAll(newStoriesTasks);

            if (newStories.Any())
            {
                // Update cache by adding new stories and removing obsolete ones
                cachedStories.AddRange(newStories.Where(story => story != null));
                cachedStories.RemoveAll(story => obsoleteStoryIds.Contains(story.Id));

                isCacheUpdated = true;
            }

            allStories = cachedStories;
        }
        else
        {
            // First time fetch
            var storyDetailsTasks = currentStoryIds.Select(GetStoryDetailsAsync);
            var stories = await Task.WhenAll(storyDetailsTasks);
            allStories = stories.Where(story => story != null).ToList();
            isCacheUpdated = true;
        }

        if (isCacheUpdated)
        {
            _memoryCache.Set(cacheKey, allStories);
        }

        return allStories;
    }

    private async Task<Story?> GetStoryDetailsAsync(int id)
    {
        var client = _httpClientFactory.CreateClient();
        try
        {
            var response = await client.GetAsync($"{_baseUrl}/item/{id}.json");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            Story? storyDetails = JsonSerializer.Deserialize<Story>(jsonResponse, jsonSerializerOptions);

            return storyDetails;
        }
        catch (HttpRequestException e)
        {
            throw new Exception($"Could not retrieve story details for Id {id} from Hacker News", e);
        }
    }

    public async Task RefreshCacheAsync()
    {
        var stories = await GetNewStoriesAsync();
        _memoryCache.Set("new-stories", stories);
        _logger.LogInformation("Cache updated with new stories at: {time}", DateTimeOffset.Now);
    }
}