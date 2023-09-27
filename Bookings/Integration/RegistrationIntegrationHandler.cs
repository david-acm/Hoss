using Eventuous;
using EventHandler = Eventuous.Subscriptions.EventHandler;

namespace Bookings.Integration;

public class RegistrationIntegrationHandler : EventHandler {
  public static readonly StreamName Stream = new("RegistrationsIntegration");

  public RegistrationIntegrationHandler(ILogger<RegistrationIntegrationHandler> logger) {
    On<ProfileEvents.RegistrationRecorded>(ctx => {
      logger.LogInformation($"ℹ️ Player {ctx.Message.DisplayName} wiht email {ctx.Message.DisplayName} ready to join games");
      return ValueTask.CompletedTask;
    });
  }
}

public static class ProfileEvents {
  [EventType("RegistrationRecorded")]
    
  public record RegistrationRecorded(string Email, string DisplayName);
}