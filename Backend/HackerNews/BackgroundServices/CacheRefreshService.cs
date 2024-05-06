using HackerNews.Application.Interfaces;

namespace HackerNews.Application.BackgroundServices;

public class CacheRefreshService : BackgroundService
{
    private readonly ILogger<CacheRefreshService> _logger;
    private readonly IHackerNewsService _hackerNewsService;
    private IConfiguration _configuration;

    private static int _cacheRefreshTime;

    public CacheRefreshService(ILogger<CacheRefreshService> logger, IHackerNewsService hackerNewsService, IConfiguration configuration)
    {
        _logger = logger;
        _hackerNewsService = hackerNewsService;
        _configuration = configuration;
        _cacheRefreshTime = _configuration.GetValue<int>("BackgroundServicesSettings:CacheRefreshTime")!;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Cache Refresher Service running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_cacheRefreshTime, stoppingToken);
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await _hackerNewsService.RefreshCacheAsync();
        }
    }
}