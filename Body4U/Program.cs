using Body4U.Membership.Api.Extensions;
using Body4U.Membership.Api.Middleware;
using Body4U.Membership.Application;
using Body4U.Membership.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services
    .AddApplication()
    .AddInfrastructure(configuration)
    .AddControllers();

services.AddEndpointsApiExplorer()
    .ConfigureSwagger();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
