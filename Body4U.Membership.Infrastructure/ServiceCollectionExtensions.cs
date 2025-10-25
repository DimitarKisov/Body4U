using Body4U.Membership.Application.Repositories;
using Body4U.Membership.Infrastructure.Persistence;
using Body4U.Membership.Infrastructure.Repositories;
using Body4U.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Body4U.Membership.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDbContext<MembershipDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(MembershipDbContext).Assembly.FullName)));

            services
                .AddScoped<IMemberRepository, MemberRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
