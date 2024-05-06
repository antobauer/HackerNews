using HackerNews.Application.Interfaces;

namespace HackerNews.Application.Domain;
public class AppStartLogic : IAppStartLogic
{
    private readonly IHackerNewsService _service;
    private readonly IConfiguration _configuration;
    private static int _defaultPageSize;

    public AppStartLogic(IHackerNewsService service, IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;
        _defaultPageSize = _configuration.GetValue<int>("PaginationSettings:BaseUrl")!;
    }

    public async Task Start()
    {
        await _service.GetNewStoriesAsync(1, _defaultPageSize, "");
    }
}