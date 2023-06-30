using Confluent.Kafka;
using BrowserHistoryStreaming.Common;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace BrowserHistoryStreaming.Producer;

public class BrowserHistoryProducerService : BackgroundService
{
    private readonly ILogger<BrowserHistoryProducerService> _logger;
    private readonly string _topic;
    private readonly string _datasetName;
    private IProducer<string, BrowserHistoryItem> _kafkaProducer;

    public BrowserHistoryProducerService(ILogger<BrowserHistoryProducerService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _topic = configuration["Kafka:BrowserHistoryTopicName"];
        _datasetName = configuration["DatasetName"];
        var config = new ProducerConfig()
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            Partitioner = Partitioner.Murmur2
        };

        _kafkaProducer = new ProducerBuilder<string, BrowserHistoryItem>(config)
            .SetValueSerializer(new BrowserHistoryItemSerializer())
            .Build();
    }

    public override void Dispose()
    {
        _kafkaProducer.Dispose();

        base.Dispose();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return StartReadingDatasetAsync(stoppingToken); 
    }

    private async Task StartReadingDatasetAsync(CancellationToken cancellationToken)
    {
        using (var reader = new StreamReader($"dataset//{ _datasetName }"))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = "," }))
        {
            csv.Context.RegisterClassMap<CsvBrowserHistoryItemMap>();
            _logger.LogInformation("Started processing of dataset.");
            var totalStatsCount = 0;
            var failedStatsCount = 0;
            while(await csv.ReadAsync())
            {
                try
                {
                    var item = csv.GetRecord<CsvBrowserHistoryItem>();
                    _logger.LogDebug($"New history item: '{ item.Id }', '{ item.Date }', '{ item.Time }', '{ item.Title }', '{ item.Url }'.");
                    _kafkaProducer.Produce(_topic, new Message<string, BrowserHistoryItem> { Key = item.GetHostFromUrl(), Value = new BrowserHistoryItem() { Title = item.Title, Url = item.Url } });                        
                }
                catch (Exception ex)
                {
                    // Ignore invalid records.
                    failedStatsCount++;
                    _logger.LogError(ex, string.Empty);
                }
                finally { totalStatsCount++; } 
            }

            _kafkaProducer.Flush(cancellationToken);
            _logger.LogInformation($"Finished processing browser hisotry. Total count: '{ totalStatsCount }'. Failed count: '{ failedStatsCount }'.");
        }
    }
}