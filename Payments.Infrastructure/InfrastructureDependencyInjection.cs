using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Payments.Application.Common.Interfaces;
using Payments.Domain.Entities;
using Payments.Infrastructure.Authentication.PasswordHasher;
using Payments.Infrastructure.Authentication.TokenGenerator;
using Payments.Infrastructure.Common.Persistence;
using Payments.Infrastructure.Repositories;
using Payments.Infrastructure.Services;
using System.Text;

namespace Payments.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddDatabase(cfg)
                    .AddUnitOfWork()
                    //.AddDistributedLocking(cfg)
                    .AddCaching()
                    .AddCatalogsAndStores()
                    .AddRepositories()
                    .AddAuthentication(cfg);

            return services;
        }

        private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration cfg)
        {
            var cs = cfg.GetConnectionString("ConnectionString")
                ?? throw new InvalidOperationException("Missing connection string: 'ConnectionString'");

            services.AddDbContextPool<AppDbContext>(opt =>
                opt.UseNpgsql("Host=localhost;Port=5432;Database=payments;Username=admin;Password=secret;Pooling=true;Minimum Pool Size=10;"), poolSize: 64);

            services.BuildServiceProvider().GetService<AppDbContext>()!.Database.Migrate();

            return services;
        }

        private static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
            return services;
        }

        //private static IServiceCollection AddDistributedLocking(this IServiceCollection services, IConfiguration cfg)
        //{
        //    var cs = cfg.GetConnectionString("distributed-locking")
        //        ?? cfg.GetConnectionString("ConnectionString")
        //        ?? throw new InvalidOperationException("Missing connection string for distributed locking");

        //    services.AddSingleton<IDistributedLockProvider>(_ =>
        //        new PostgresDistributedSynchronizationProvider(cs));

        //    services.AddScoped<IDistributedOrderLock, PostgresDistributedOrderLock>();

        //    return services;
        //}

        private static IServiceCollection AddCaching(this IServiceCollection services)
        {
            services.AddMemoryCache(opt => { opt.SizeLimit = 1024; });
            return services;
        }

        private static IServiceCollection AddCatalogsAndStores(this IServiceCollection services)
        {
            services.AddScoped<ICurrencyCatalog, CurrencyCatalog>();
            services.AddScoped<IIdempotencyStore, IdempotencyStore>();
            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IReadRepository<Order>, OrdersReadRepository>();
            services.AddScoped<IOrdersWriteRepository, OrdersWriteRepository>();
            services.AddScoped<IReadRepository<Payment>, PaymentsReadRepository>();
            services.AddScoped<IWriteRepository<Payment>, PaymentsWriteRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            return services;
        }

        private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration cfg)
        {
            var jwtSettings = new JwtSettings();
            cfg.Bind(JwtSettings.Section, jwtSettings);

            services.AddSingleton(Options.Create(jwtSettings));
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            });

            return services;
        }

    }
}
