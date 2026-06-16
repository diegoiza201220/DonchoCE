using WebApiDonCho.Services;

namespace WebApiDonCho
{
    public static class AppStartup
    {
        public static async Task EjecutarAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var sp = scope.ServiceProvider;

            // 1. Primero valida — si falla, la app no sube
            sp.GetRequiredService<DailyConfigurationValidatorService>().ConfigurarIVA();

            // 2. Luego calienta caché
            //await sp.GetRequiredService<CacheWarmupService>().CalentarAsync();
        }
    }
}
