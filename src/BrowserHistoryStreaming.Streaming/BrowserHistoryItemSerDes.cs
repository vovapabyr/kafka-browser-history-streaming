using System.Text.Json;
using BrowserHistoryStreaming.Common;
using Confluent.Kafka;
using Streamiz.Kafka.Net.SerDes;

namespace BrowserHisotrySyteaming.Streaming;

public class BrowserHistoryItemSerDes : AbstractSerDes<BrowserHistoryItem>
{
    public override BrowserHistoryItem Deserialize(byte[] data, SerializationContext context)
    {
        return JsonSerializer.Deserialize<BrowserHistoryItem>(data);
    }

    public override byte[] Serialize(BrowserHistoryItem data, SerializationContext context)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data);
    }
}