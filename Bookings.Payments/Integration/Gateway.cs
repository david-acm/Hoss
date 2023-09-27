using Eventuous;
using Eventuous.EventStore.Producers;
using Eventuous.Gateway;
using Eventuous.Subscriptions.Context;
using static Bookings.Payments.Domain.PaymentEvents;
using static Bookings.Payments.Domain.ProfileEvents;
using static Bookings.Payments.Integration.IntegrationEvents;

namespace Bookings.Payments.Integration;

public static class EventExtensions
{
  public static ValueTask<GatewayMessage<EventStoreProduceOptions>[]> PublishWith<TEvent, TPublishedEvent>(
    this TEvent evt, StreamName stream, TPublishedEvent publishedEvent)
    where TPublishedEvent : class
  {
    return ValueTask.FromResult(new[]
    {
      new GatewayMessage<EventStoreProduceOptions>(
        stream,
        publishedEvent,
        new Metadata(),
        new EventStoreProduceOptions())
    });
  }
}

public static class Gateway
{
  public static ValueTask<GatewayMessage<EventStoreProduceOptions>[]> Transform(IMessageConsumeContext original) =>
    original.Message switch
    {
      ProfileRegistered e => e.PublishWith(
        new StreamName("RegistrationsIntegration"),
        new RegistrationRecorded(e.Email, e.DisplayName)),

      PaymentRecorded e => e.PublishWith(
        new StreamName("PaymentsIntegration"),
        new BookingPaymentRecorded(e.PaymentId, e.BookingId, e.Amount, e.Currency)),

      _ => ValueTask.FromResult(Array.Empty<GatewayMessage<EventStoreProduceOptions>>())
    };
}

public static class IntegrationEvents
{
  [EventType("BookingPaymentRecorded")]
  public record BookingPaymentRecorded(string PaymentId, string BookingId, float Amount, string Currency);


  [EventType("RegistrationRecorded")]
  public record RegistrationRecorded(string Email, string DisplayName);
}