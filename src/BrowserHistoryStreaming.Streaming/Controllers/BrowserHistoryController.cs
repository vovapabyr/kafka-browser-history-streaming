using Microsoft.AspNetCore.Mvc;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.State;

namespace BrowserHisotrySyteaming.Streaming;

[ApiController]
[Route("[controller]")]
public class BrowserHistoryController : ControllerBase
{
    private readonly BrowserHistoryStreamBuilderService _streamBuilderService;

    public BrowserHistoryController(BrowserHistoryStreamBuilderService streamBuilderService)
    {
        _streamBuilderService = streamBuilderService;
    }

    [HttpGet("topVisited")]
    public IActionResult GetTopVisistedDomains(int count = 5)
    {
        return Ok(_streamBuilderService.Stream.Store(StoreQueryParameters.FromNameAndType("domain-counts", QueryableStoreTypes.KeyValueStore<string, long>())).All().OrderByDescending((p) => p.Value).Take(count));
    }
}