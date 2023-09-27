using Bookings.Payments.Domain;
using Eventuous.AspNetCore.Web;

namespace Bookings.Payments.Application;

public static class V1
{
  public static class ProfileCommands
  {
    [HttpCommand<Profile>(Route = "/registrations")]
    public record RegisterProfileHttp(
      string ProfileId,
      string Email,
      string DisplayName);

    public record RegisterProfile
    (
      ProfileId ProfileId,
      ProfileEmail ProfileEmail,
      DisplayName DisplayName
    );

    public record ProfileEmail(string Email);

    public record DisplayName(string Name);
  }
}