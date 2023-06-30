using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Stream;
using BrowserHistoryStreaming.Common;
using Streamiz.Kafka.Net.Table;

namespace BrowserHisotrySyteaming.Streaming;

public class BrowserHistoryStreamingService : BackgroundService
{
    private readonly ILogger<BrowserHistoryStreamingService> _logger;
    private KafkaStream _stream;

    public BrowserHistoryStreamingService(ILogger<BrowserHistoryStreamingService> logger, IConfiguration configuration)
    {
        _logger = logger;
        var statsTopic = configuration["Kafka:BrowserHistoryTopicName"];
        var peaksTopic = configuration["Kafka:BrowserHistoryCountTopicName"];

        var streamConfig = new StreamConfig<StringSerDes, BrowserHistoryItemSerDes>()
        {
            ApplicationId = "domain-counters",
            BootstrapServers = configuration["Kafka:BootstrapServers"]
        };
        var streamTopology = BuildBrowserHistoryCountTopology(statsTopic, peaksTopic);
        _stream = new KafkaStream(streamTopology, streamConfig);
    }

    public override void Dispose()
    {
        _stream.Dispose();
        base.Dispose();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return StartStatsProcessingAsync(stoppingToken);
    }

    private async Task StartStatsProcessingAsync(CancellationToken cancellationToken)
    {
        await _stream.StartAsync(cancellationToken);

        _logger.LogInformation("Processing finished");
    }

    private Topology BuildBrowserHistoryCountTopology(string inputTopic, string outputTopic)
    {
        var streamBuilder = new StreamBuilder();
        streamBuilder.Stream<string, BrowserHistoryItem>(inputTopic)
            .Peek((k, v) => _logger.LogDebug($"Processing Key: '{ k }', Value: '{ v }'."))
            .GroupByKey()
            .Count(InMemory.As<string, long>("domain-counts"))
            .ToStream()
            .Peek((k, v) => _logger.LogDebug($"Key: '{ k }', Count: '{ v }'."))
            .To<StringSerDes, LongSerDes>(outputTopic);

        return streamBuilder.Build();
    }
}