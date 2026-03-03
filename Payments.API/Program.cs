using Payments.Infrastructure;
using Payments.Application;
using Scalar.AspNetCore;
using Payments.API;


var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UsePresentation();

app.Run();