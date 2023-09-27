using Eventuous;

namespace Bookings.Payments.Domain;

public static class ProfileEvents
{
  [EventType("ProfileRegistered")]
  public record ProfileRegistered(string Id, string Email, string DisplayName);
}