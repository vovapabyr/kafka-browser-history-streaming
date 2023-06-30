using Confluent.Kafka;
using Streamiz.Kafka.Net.SerDes;

namespace BrowserHisotrySyteaming.Streaming;

// Same implementation of org.apache.kafka.common.serialization.LongDeserializer https://github.com/a0x8o/kafka/blob/master/clients/src/main/java/org/apache/kafka/common/serialization/LongSerializer.java#L32
// If we use Int64SerDes we cannot read with kafka-console-consumer.sh
public class LongSerDes : AbstractSerDes<long>
{
    public override long Deserialize(byte[] data, SerializationContext context)
    {
        throw new NotImplementedException();
    }

    public override byte[] Serialize(long data, SerializationContext context)
    {
        return (byte[]) (Array)new sbyte[] {
            (sbyte) (data >>> 56),
            (sbyte) (data >>> 48),
            (sbyte) (data >>> 40),
            (sbyte) (data >>> 32),
            (sbyte) (data >>> 24),
            (sbyte) (data >>> 16),
            (sbyte) (data >>> 8),
            unchecked((sbyte)data)
        };
    }
}