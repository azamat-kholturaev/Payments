using FluentValidation;
using MediatR;
//using Payments.Application.Common.Concurrency;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payments.Application.Common.Behaviors;


namespace Payments.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration cfg)
        {
            //services.AddConcurrencyStrateg(cfg);

            services.AddMediatR(options => options.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            return services;
        }


        //private static IServiceCollection AddConcurrencyStrateg(this IServiceCollection services, IConfiguration cfg)
        //{
        //    services.AddScoped<ForUpdateConcurrencyStrategy>();
        //    services.AddScoped<DistributedLockConcurrencyStrategy>();

        //    services.AddScoped<IPaymentConcurrencyStrategy>(sp =>
        //    {
        //        var cfg = sp.GetRequiredService<IConfiguration>();
        //        var mode = cfg["Payments:ConcurrencyMode"]; // "for-update" | "distributed-lock"

        //        return mode == "distributed-lock"
        //            ? sp.GetRequiredService<DistributedLockConcurrencyStrategy>()
        //            : sp.GetRequiredService<ForUpdateConcurrencyStrategy>();
        //    });

        //    return services;
        //}
    }
}
