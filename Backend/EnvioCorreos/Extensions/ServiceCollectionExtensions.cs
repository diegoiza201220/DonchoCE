using EnvioCorreos.Configuration;
using EnvioCorreos.Interfaces;
using EnvioCorreos.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EnvioCorreos.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEnvioCorreos(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            services.Configure<EmailOptions>(
                configuration.GetSection(EmailOptions.SectionName));

            services.AddScoped<IEmailService, EmailService>();
            return services;
        }

        // Desde código
        public static IServiceCollection AddEnvioCorreosManual(
            this IServiceCollection services,
            Action<EmailOptions> configure)
        {
            services.Configure(configure);
            services.AddScoped<IEmailService, EmailService>();
            return services;
        }
    }
}