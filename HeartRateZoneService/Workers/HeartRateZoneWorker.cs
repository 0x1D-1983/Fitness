using Confluent.Kafka;
using HeartRateZoneService.Domain;
using static Confluent.Kafka.ConfigPropertyNames;

namespace HeartRateZoneService.Workers;

public class HeartRateZoneWorker : BackgroundService
{
    private const string BiometricsImportedTopicName = "BiometricsImported";
    private const string HeartRateZoneReachedTopicName = "HeartRateZoneReached";
    private readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    private readonly IConsumer<String, Biometrics> _consumer;
    private readonly IProducer<String, HeartRateZoneReached> _producer;
    private readonly ILogger<HeartRateZoneWorker> _logger;

    public HeartRateZoneWorker(
        IConsumer<String, Biometrics> consumer,
        IProducer<String, HeartRateZoneReached> producer,
        ILogger<HeartRateZoneWorker> logger)
    {
        _consumer = consumer;
        _producer = producer;
        _logger = logger;

        logger.LogInformation("HeartRateZoneWorker is Active.");
    }

    protected virtual async Task HandleMessage(Biometrics biometrics, CancellationToken token)
    {
        _logger.LogInformation($"Message received: {biometrics.DeviceId}");

        _producer.BeginTransaction();

        var offsets = _consumer.Assignment.Select(topicPartition => new TopicPartitionOffset(topicPartition,
            _consumer.Position(topicPartition)));

        _producer.SendOffsetsToTransaction(offsets, _consumer.ConsumerGroupMetadata, DefaultTimeout);

        var heartRateZoneReachedMessageTasks = biometrics.HeartRates
            .Select(heartRate => new HeartRateZoneReached(biometrics.DeviceId, heartRate.GetHeartRateZone(biometrics.MaxHeartRate),
            heartRate.DateTime, heartRate.Value, biometrics.MaxHeartRate))
            .Where(heartRateZoneReached => heartRateZoneReached.Zone != HeartRateZone.None)
            .Select(async heartRateZoneReached =>
            {
                await _producer.ProduceAsync(HeartRateZoneReachedTopicName, new Message<string, HeartRateZoneReached>
                {
                    Key = biometrics.DeviceId.ToString(),
                    Value = heartRateZoneReached
                }, token);
            });

        try
        {
            await Task.WhenAll(heartRateZoneReachedMessageTasks);
            _producer.CommitTransaction();
        }
        catch (Exception ex)
        {
            _producer.AbortTransaction();
            string error = $"Transacton failed for Device ID {biometrics.DeviceId}";
            _logger.LogError(error);
            throw new Exception(error, ex);
        }

    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        _producer.InitTransactions(DefaultTimeout);
        _consumer.Subscribe(BiometricsImportedTopicName);

        while (!token.IsCancellationRequested)
        {
            var result = _consumer.Consume(token);
            await HandleMessage(result.Message.Value, token);
        }

        _consumer.Close();
    }   
}
