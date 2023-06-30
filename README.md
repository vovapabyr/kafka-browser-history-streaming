# Browser Visiting Statistics With Kafka Streaming 
## About
```./kafka-console-consumer.sh --topic browser-history-count-topic --from-beginning --bootstrap-server localhost:19092  --formatter kafka.tools.DefaultMessageFormatter --property print.key=true  --property print.value=true --property key.deserializer=org.apache.kafka.common.serialization.StringDeserializer --property value.deserializer=org.apache.kafka.common.serialization.LongDeserializer```






 
