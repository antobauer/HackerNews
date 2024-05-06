using HackerNews.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HackerNewsController : Controller
{
    private readonly IHackerNewsService _hackerNewsService;

    public HackerNewsController(IHackerNewsService service)
    {
        _hackerNewsService = service;
    }

    [HttpGet("newstories")]
    public async Task<IActionResult> GetNewStories(int page, int pageSize, string? searchFilter)
    {
        try
        {
            var newStories = await _hackerNewsService.GetNewStoriesAsync(page, pageSize, searchFilter);
            return Ok(newStories);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while fetching new stories.");
        }
    }
}