using System.Text.Json;
using Confluent.Kafka;

namespace BrowserHistoryStreaming.Common;

public class BrowserHistoryItemSerializer : ISerializer<BrowserHistoryItem>
{
    public byte[] Serialize(BrowserHistoryItem data, SerializationContext context)
    {
        return JsonSerializer.SerializeToUtf8Bytes(data);
    }
}