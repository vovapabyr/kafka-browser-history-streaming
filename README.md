# Browser Visiting Statistics With Kafka Streaming 
## About
This project aims to see the most visited domains on the browser. For browser history export I used the google chrome [extension](https://chrome.google.com/webstore/detail/export-chrome-history/dihloblpkeiddiaojbagoecedbfpifdj). Google Chrome allows to export history only for the last 90 days.
## How to run
Before running the project you need to bind mount the directory with the browser history dataset on your host computer to the directory producer uses [docker-compose.yml](https://github.com/vovapabyr/kafka-browser-history-streaming/blob/main/docker-compose.yml):
```
  browserhistorystreamingproducer:
    image: browserhistorystreamingproducer
    container_name: browserhistorystreamingproducer
    build:
      context: .
      dockerfile: src/BrowserHistoryStreaming.Producer/Dockerfile
    ports:
      - 5222:5222
    volumes:
      - "./data/dataset:/app/dataset"
    depends_on:
      init-kafka: 
        condition: service_completed_successfully
```
After that go to the root of the project and run ```docker compose up``` command.
## Results
After the dataset is fully processed navigate to the ```/browserHistory/topVisited?count=5``` URL of the streaming application to see the top 5 (change count to see more) visited domains:
![history_top_5](https://github.com/vovapabyr/kafka-browser-history-streaming/assets/25819135/78eceaf2-9c66-41d3-941b-5ca056ad7655)
We read the top visited domains from the local state store ```domain-counts``` of the streaming application ([link](https://github.com/vovapabyr/kafka-browser-history-streaming/blob/main/src/BrowserHistoryStreaming.Streaming/Controllers/BrowserHistoryController.cs)). If we want to process dataset in ex. 3 parallel application instances, we would need to create 3 partitions in the ```browser-history-topic``` [docker-compose.yml](https://github.com/vovapabyr/kafka-browser-history-streaming/blob/main/docker-compose.yml):
```
   echo -e 'Creating kafka browser-history topic'
   kafka-topics --bootstrap-server kafka-1:9092 --create --if-not-exists --topic browser-history-topic --replication-factor 3 --partitions 3
```
and run ```docker compose up --scale browserhistorystreaming=3```.
But, there is going to be the problem of getting top 5 visited domains, as each application instance would have its own state store with its own set of data. So, asking any of the instances would return only top 5 visited domains of the specific partition. One of the solutions is to create GlobalKTable for the output topic ```browser-history-count-topic```, then querying this global state store should return the top 5 domains across all partitions.

Also, to read the data from the final ```browser-history-count-topic``` topic, we can run kafka console consumer:  
```./kafka-console-consumer.sh --topic browser-history-count-topic --from-beginning --bootstrap-server localhost:19092  --formatter kafka.tools.DefaultMessageFormatter --property print.key=true  --property print.value=true --property key.deserializer=org.apache.kafka.common.serialization.StringDeserializer --property value.deserializer=org.apache.kafka.common.serialization.LongDeserializer```

which represents the stream of changes of the domain counts:
![kafka_console_consumer_output_topic](https://github.com/vovapabyr/kafka-browser-history-streaming/assets/25819135/018b5641-064d-4472-b189-3a63021637d9)







 
