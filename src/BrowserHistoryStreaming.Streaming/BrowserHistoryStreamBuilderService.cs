using BrowserHistoryStreaming.Common;
using Confluent.Kafka;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Stream;
using Streamiz.Kafka.Net.Table;

namespace BrowserHisotrySyteaming.Streaming;

public class BrowserHistoryStreamBuilderService : IDisposable
{
    private readonly ILogger<BrowserHistoryStreamBuilderService> _logger;
    

    public BrowserHistoryStreamBuilderService(ILogger<BrowserHistoryStreamBuilderService> logger, IConfiguration configuration)
    {
        _logger = logger;
        var historyTopic = configuration["Kafka:BrowserHistoryTopicName"];
        var historyCountTopic = configuration["Kafka:BrowserHistoryCountTopicName"];

        var streamConfig = new StreamConfig<StringSerDes, BrowserHistoryItemSerDes>()
        {
            ApplicationId = "domain-counters",
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        var streamTopology = BuildBrowserHistoryCountTopology(historyTopic, historyCountTopic);
        Stream = new KafkaStream(streamTopology, streamConfig);
        BuildBrowserHistoryCountTopology(historyTopic, historyCountTopic);
    }

    public KafkaStream Stream { get; }

    public void Dispose()
    {
        _logger.LogInformation("Stream processing finished. Disposing stream.");

        Stream.Dispose();
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