using Eventuous;

namespace Bookings.Payments.Domain;

public class Profile : Aggregate<ProfileState>
{
  public void Register(string commandProfileId, string commandEmail, string commandDisplayName) 
    => Apply(new ProfileEvents.ProfileRegistered(commandProfileId, commandEmail, commandDisplayName));
}

public record ProfileState : State<ProfileState>
{
  public string Email { get; init; }

  public string Id { get; init; }

  public string DisplayName { get; init; }

  public ProfileState()
  {
    On<ProfileEvents.ProfileRegistered>((state, registered) => state with
    {
      Email = registered.Email,
      Id = registered.Id,
      DisplayName = registered.DisplayName
    });
  }
}

public record ProfileId(string value) : AggregateId(value);
