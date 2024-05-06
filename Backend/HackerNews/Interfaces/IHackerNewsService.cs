using HackerNews.Application.Models;
using HackerNews.Application.Models.Paging;

namespace HackerNews.Application.Interfaces;

public interface IHackerNewsService
{
    Task<PaginatedList<Story>> GetNewStoriesAsync(int page, int pageSize, string searchFilter);

    Task RefreshCacheAsync();
}