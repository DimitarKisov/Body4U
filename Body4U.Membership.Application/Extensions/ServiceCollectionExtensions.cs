namespace Body4U.Membership.Application
{
    using Body4U.Membership.Application.Behaviors;
    using FluentValidation;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using System.Reflection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                .AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            return services;
        }
    }
}