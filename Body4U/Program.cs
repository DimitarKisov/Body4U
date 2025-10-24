using Body4U.Membership.Application.Behaviors;
using Body4U.Membership.Application.Repositories;
using Body4U.Membership.Infrastructure;
using Body4U.Membership.Infrastructure.Persistence;
using Body4U.SharedKernel.Domain;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// Database
services.AddDbContext<MembershipDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MembershipDb"),
        x => x.MigrationsAssembly("Body4U.Membership.Infrastructure")
    )
);

// MediatR
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(IUnitOfWork).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// FluentValidation
services.AddValidatorsFromAssembly(typeof(IUnitOfWork).Assembly);

// Repositories
services.AddInfrastructure(configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
