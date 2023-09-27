using Bookings.Payments;
using Bookings.Payments.Domain;
using Bookings.Payments.Infrastructure;
using Eventuous;
using Eventuous.AspNetCore;
using Serilog;
using static Bookings.Payments.Application.V1.ProfileCommands;

TypeMap.RegisterKnownEventTypes();
Logging.ConfigureLog();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// OpenTelemetry instrumentation must be added before adding Eventuous services
builder.Services.AddTelemetry();

builder.Services.AddServices(builder.Configuration);
builder.Host.UseSerilog();

var app = builder.Build();
app.AddEventuousLogs();

app.UseSwagger();
app.UseOpenTelemetryPrometheusScrapingEndpoint();

// Here we discover commands by their annotations
// app.MapDiscoveredCommands();
// app.MapDiscoveredCommands<Payment>();
app.MapDiscoveredCommands();

app.MapAggregateCommands<Profile>()
  .MapCommand(RegisterProfileFromRegistrations());

// TODO: Change gateway to publish a game start or join event
/* TODO: For join extended version
 1. Define DTO record as command/api. What are we going to receive?
 2. Add Command Service handler. Do we need to create or update?
 3. Add Aggregate behavior. Can we do it? (Preconditions)
 4. Perform State transition. What would change?
 */


/* TODO: For join extended version
 1. Define DTO record
 2. Define Command
 3. Create Map
 4. Add Command Service
 5. Add Aggregate behavior
 
 
   1. Define DTO record as command/api. What are we going to receive?
   - Decouple API from command
   - Map DTO to Command
   - Validate Command
   
   2. Add Command Service handler. Do we need to create or update?
   
   3. Add Aggregate behavior. Can we do it?
   - Preconditions
   - Branched Business Logic
   - Ensure Post conditions?
   - What else can happen? (And not so happy paths?)
   
   4. Perform State transition(s). What would change?
 */




app.UseSwaggerUI();

app.Run();

ConvertAndEnrichCommand<RegisterProfileHttp, RegisterProfile> RegisterProfileFromRegistrations() =>
  (req, _) =>
    new RegisterProfile(
      new ProfileId(req.ProfileId),
      new ProfileEmail(req.Email),
      new DisplayName(req.DisplayName));