const string corsRoleName = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.AddArchitecture(corsRoleName);

var app = builder.Build();

app.UseArchitectures(corsRoleName);

app.Run();


