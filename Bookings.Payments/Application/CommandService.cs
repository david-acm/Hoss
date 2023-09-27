using System.Text.Json.Serialization;
using Bookings.Payments.Domain;
using Eventuous;
using Eventuous.AspNetCore.Web;

namespace Bookings.Payments.Application;

public class CommandService : CommandService<Payment, PaymentState, PaymentId>
{
  public CommandService(IAggregateStore store) : base(store)
  {
    OnNew<PaymentCommands.RecordPayment>(
      cmd => new PaymentId(cmd.PaymentId),
      (payment, cmd) => payment.ProcessPayment(
        new PaymentId(cmd.PaymentId),
        cmd.BookingId,
        new Money(cmd.Amount, cmd.Currency),
        cmd.Method,
        cmd.Provider
      )
    );
  }
}

[AggregateCommands<Payment>]
public static class PaymentCommands {
  // [HttpCommand]
  [HttpCommand(Route = "payments")]
  public record RecordPayment(
    string PaymentId,
    string BookingId,
    float Amount,
    string Currency,
    string Method,
    string Provider,
    [property: JsonIgnore] string PaidBy
  );
}