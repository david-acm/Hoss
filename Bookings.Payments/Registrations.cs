using Bookings.Payments.Application;
using Bookings.Payments.Domain;
using Bookings.Payments.Infrastructure;
using Bookings.Payments.Integration;
using Eventuous.Diagnostics.OpenTelemetry;
using Eventuous.EventStore;
using Eventuous.EventStore.Producers;
using Eventuous.EventStore.Subscriptions;
using Eventuous.Producers;
using Eventuous.Projections.MongoDB;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Bookings.Payments;

public static class Registrations {
    public static void AddServices(this IServiceCollection services, IConfiguration configuration) {
        services.AddEventStoreClient(configuration["EventStore:ConnectionString"]!);
        services.AddAggregateStore<EsdbEventStore>();
        services.AddCommandService<CommandService, Payment>();
        services.AddSingleton(Mongo.ConfigureMongo(configuration));
        services.AddCheckpointStore<MongoCheckpointStore>();
        services.AddEventProducer<EventStoreProducer>();

        services
            .AddGateway<AllStreamSubscription, AllStreamSubscriptionOptions, EventStoreProducer>(
                "IntegrationSubscription",
                PaymentsGateway.Transform
            );
    }

    public static void AddTelemetry(this IServiceCollection services) {
        services.AddOpenTelemetry()
            .WithMetrics(
                builder => builder
                    .AddAspNetCoreInstrumentation()
                    .AddEventuous()
                    .AddEventuousSubscriptions()
                    .AddPrometheusExporter()
            );

        services.AddOpenTelemetry()
            .WithTracing(
                builder => builder
                    .AddAspNetCoreInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddEventuousTracing()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("payments"))
                    .SetSampler(new AlwaysOnSampler())
                    .AddZipkinExporter()
            );
    }
}
