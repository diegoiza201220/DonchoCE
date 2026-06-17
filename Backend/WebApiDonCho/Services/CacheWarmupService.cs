using Microsoft.Extensions.Caching.Memory;

namespace WebApiDonCho.Services
{
    public class CacheWarmupService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheWarmupService> _logger;

        public CacheWarmupService(IMemoryCache cache, ILogger<CacheWarmupService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task CalentarAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Calentando caché...");

            await Task.WhenAll(
                CargarTiposComprobantesAsync(ct),
                CargarParametrosGeneralesAsync(ct),
                CargarClientesFrecuentesAsync(ct)
            );

            _logger.LogInformation("✔ Caché lista.");
        }

        private async Task CargarTiposComprobantesAsync(CancellationToken ct)
        {
            //var datos = await _db.TiposComprobantes
            //    .AsNoTracking()
            //    .ToListAsync(ct);

            //_cache.Set("cat:tipos_comprobantes", datos,
            //    new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12) });
        }

        private async Task CargarParametrosGeneralesAsync(CancellationToken ct)
        {
            //var datos = await _db.Parametros
            //    .AsNoTracking()
            //    .ToDictionaryAsync(p => p.Clave, p => p.Valor, ct);

            //_cache.Set("cat:parametros", datos,
            //    new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(6) });
        }

        private async Task CargarClientesFrecuentesAsync(CancellationToken ct)
        {
            //var datos = await _db.Clientes
            //    .AsNoTracking()
            //    .Where(c => c.Activo)
            //    .OrderByDescending(c => c.UltimaTransaccion)
            //    .Take(500)
            //    .ToListAsync(ct);

            //_cache.Set("cat:clientes_frecuentes", datos,
            //    new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
        }
    }
}
