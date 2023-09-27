using Bookings.Payments.Domain;
using Eventuous;
using static Bookings.Payments.Application.V1.ProfileCommands;

namespace Bookings.Payments.Application;

public class ProfileService : CommandService<Profile, ProfileState, ProfileId>
{
  public ProfileService(IAggregateStore store)
    : base(store)
  {
    OnNew<RegisterProfile>(
      cmd => new ProfileId(cmd.ProfileId),
      (profile, cmd) => profile.Register(
        cmd.ProfileId,
        cmd.ProfileEmail.Email,
        cmd.DisplayName.Name));
  }
}