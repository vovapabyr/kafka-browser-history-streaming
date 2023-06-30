using Streamiz.Kafka.Net;

namespace BrowserHisotrySyteaming.Streaming;

public class BrowserHistoryStreamingService : BackgroundService
{
    private readonly BrowserHistoryStreamBuilderService _streamBuilderService;
    private readonly ILogger<BrowserHistoryStreamingService> _logger;

    public BrowserHistoryStreamingService(ILogger<BrowserHistoryStreamingService> logger, IConfiguration configuration, BrowserHistoryStreamBuilderService streamBuilderService)
    {
        _logger = logger;
        _streamBuilderService = streamBuilderService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return StartStatsProcessingAsync(stoppingToken);
    }

    private Task StartStatsProcessingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting stream processing.");

        return _streamBuilderService.Stream.StartAsync(cancellationToken);
    }
}