using Microsoft.Extensions.Caching.Memory;

namespace WebApiDonCho.Services
{
    public class CacheWarmupService(ILogger<CacheWarmupService> logger)
    {
        private readonly ILogger<CacheWarmupService> _logger = logger;

        //public async Task CalentarAsync(CancellationToken ct = default)
        //{
        //    _logger.LogInformation("Calentando caché...");

        //    await Task.WhenAll(
        //        //CargarTiposComprobantesAsync(ct),
        //        //CargarParametrosGeneralesAsync(ct),
        //        //CargarClientesFrecuentesAsync(ct)
        //    );

        //    _logger.LogInformation("✔ Caché lista.");
        //}

        //private static async Task CargarClientesFrecuentesAsync(CancellationToken ct)
        //{
        //    //var datos = await _db.Clientes
        //    //    .AsNoTracking()
        //    //    .Where(c => c.Activo)
        //    //    .OrderByDescending(c => c.UltimaTransaccion)
        //    //    .Take(500)
        //    //    .ToListAsync(ct);

        //    //_cache.Set("cat:clientes_frecuentes", datos,
        //    //    new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
        //}
    }
}
