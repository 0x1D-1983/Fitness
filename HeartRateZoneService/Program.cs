using HeartRateZoneService;
using HeartRateZoneService.Workers;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Options;
using Confluent.Kafka.SyncOverAsync;
using HeartRateZoneService.Domain;

IHost host = Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
{
    services.Configure<SchemaRegistryConfig>(hostContext.Configuration.GetSection("SchemaRegistry"));
    services.Configure<ProducerConfig>(hostContext.Configuration.GetSection("Producer"));
    services.Configure<ConsumerConfig>(hostContext.Configuration.GetSection("Consumer"));

    // schema registry
    services.AddSingleton<ISchemaRegistryClient>(sp =>
    {
        var regConfig = sp.GetRequiredService<IOptions<SchemaRegistryConfig>>();
        return new CachedSchemaRegistryClient(regConfig.Value);
    });

    // producer
    services.AddSingleton<IProducer<String, HeartRateZoneReached>>(sp =>
    {
        var config = sp.GetRequiredService<IOptions<ProducerConfig>>();

        var schemaRegistry = sp.GetRequiredService<ISchemaRegistryClient>();

        return new ProducerBuilder<String, HeartRateZoneReached>(config.Value)
            .SetValueSerializer(new JsonSerializer<HeartRateZoneReached>(schemaRegistry))
            .Build();
    });

    // consumer
    services.AddSingleton<IConsumer<String, Biometrics>>(sp =>
    {
        var config = sp.GetRequiredService<IOptions<ConsumerConfig>>();

        return new ConsumerBuilder<String, Biometrics>(config.Value)
            .SetValueDeserializer(new JsonDeserializer<Biometrics>().AsSyncOverAsync())
            .Build();
    });

    services.AddHostedService<HeartRateZoneWorker>();
}).Build();

await host.RunAsync();