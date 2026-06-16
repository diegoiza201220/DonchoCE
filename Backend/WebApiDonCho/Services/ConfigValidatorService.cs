using EFModel.DTO;
using EFModel.Interfaces;
using EFModel.Models;
using MimeKit.Tnef;
using WebApiDonCho.Utils;

namespace WebApiDonCho.Services
{
    public class DailyConfigurationValidatorService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<DailyConfigurationValidatorService> _logger;
        private readonly IUnitOfWork _uow;
        private readonly ICacheService _cache;

        public DailyConfigurationValidatorService(IConfiguration config, ILogger<DailyConfigurationValidatorService> logger, IUnitOfWork unitOfWork, ICacheService cache)
        {
            _config = config;
            _logger = logger;
            _uow = unitOfWork;
            _cache = cache;
        }

        public async Task ConfigurarIVA()
        {
            try
            {
                bool esFeriado = _uow.GenFeriadoR.GetByFecha(DateTime.Now.ToIntFecha());
                //var productos = await _uow.FacProductoR.GetAllDtoAsync();
                var lproductos = _uow.FacProductoR.GetAll();
                string parametroIvaDefault = _uow.GenParametroR.GetById("ID_CAT_DETALLE_IVA_DEFAULT").Valor;
                string parametroIvaFeriados = _uow.GenParametroR.GetById("ID_CAT_DETALLE_IVA_FERIADOS").Valor;
                GenCatalogoDetalle CdIvaDefault = _uow.GenCatalogoDetalleR.GetById(int.Parse(parametroIvaDefault));
                GenCatalogoDetalle CdIvaFeriados = _uow.GenCatalogoDetalleR.GetById(int.Parse(parametroIvaFeriados));
                string porcentajeIvaDefault = CdIvaDefault.Codigo.Replace("%", "");
                string porcentajeIvaFeriado = CdIvaFeriados.Codigo.Replace("%", "");

                decimal denom = esFeriado ? 1 + (decimal.Parse(porcentajeIvaFeriado) / 100) : 1 + (decimal.Parse(porcentajeIvaDefault) / 100);
                int idCatDetalleIva = esFeriado ? int.Parse(parametroIvaFeriados) : int.Parse(parametroIvaDefault);

                foreach (FacProducto lproducto in lproductos)
                {
                    lproducto.Valor = Math.Round(lproducto.ValorDoncho / denom, 2);
                    lproducto.CodigoIva = idCatDetalleIva;
                    _uow.FacProductoR.Update(lproducto);
                }
                _ = _uow.SaveChangesAsync();
                var productos = _uow.FacProductoR.GetAllDto();
                _cache.SetPermanent("PRODUCTOS_ALL", productos);
                _cache.SetPermanent("ES_FERIADO", esFeriado);
                _cache.SetPermanent("ID_CATDETALLE_IVA", esFeriado ? CdIvaFeriados.Id: CdIvaDefault.Id);
                _cache.SetPermanent("PORCENTAJE_IVA", esFeriado ? porcentajeIvaFeriado : porcentajeIvaDefault);
                _cache.SetPermanent("CODIGO_IVA", esFeriado ? CdIvaFeriados.Valor : CdIvaDefault.Valor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw; // Re-lanzar la excepción para que el servicio de validación diaria pueda manejarla.
            }
        }

        public void Validar()
        {
            var errores = new List<string>();

            Requerir("ConnectionStrings:DefaultConnection", errores);
            Requerir("Jwt:SecretKey", errores);
            Requerir("Sri:RucEmisor", errores);

            if (errores.Any())
            {
                foreach (var e in errores)
                    _logger.LogCritical("Configuración faltante: {Error}", e);

                throw new InvalidOperationException(
                    $"La API no puede iniciar. Faltan {errores.Count} configuración(es) requerida(s).");
            }

            _logger.LogInformation("✔ Configuraciones validadas correctamente.");
        }

        private void Requerir(string clave, List<string> errores)
        {
            if (string.IsNullOrWhiteSpace(_config[clave]))
                errores.Add(clave);
        }
    }
}
