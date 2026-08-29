using EFModel.Interfaces;
using EFModel.Models;
using Utils;


namespace WebApiDonCho.Services
{
    public class DailyConfigurationValidatorService(IConfiguration config, ILogger<DailyConfigurationValidatorService> logger, IUnitOfWork unitOfWork, ICacheService cache)
    {
        private readonly ILogger<DailyConfigurationValidatorService> _logger = logger;

        public async Task ConfigurarIVA()
        {
            try
            {
                bool esFeriado = unitOfWork.GenFeriadoR.GetByFecha(DateTime.Now.ToIntFecha());
                var lproductos = unitOfWork.FacProductoR.GetAll();
                string parametroIvaDefault = unitOfWork.GenParametroR.GetById(Constantes.ID_CAT_DETALLE_IVA_DEFAULT).Valor;
                string parametroIvaFeriados = unitOfWork.GenParametroR.GetById(Constantes.ID_CAT_DETALLE_IVA_FERIADOS).Valor;
                GenCatalogoDetalle CdIvaDefault = unitOfWork.GenCatalogoDetalleR.GetById(int.Parse(parametroIvaDefault));
                GenCatalogoDetalle CdIvaFeriados = unitOfWork.GenCatalogoDetalleR.GetById(int.Parse(parametroIvaFeriados));
                string porcentajeIvaDefault = CdIvaDefault.Codigo.Replace("%", "");
                string porcentajeIvaFeriado = CdIvaFeriados.Codigo.Replace("%", "");

                decimal denom = esFeriado ? 1 + (decimal.Parse(porcentajeIvaFeriado) / 100) : 1 + (decimal.Parse(porcentajeIvaDefault) / 100);
                int idCatDetalleIva = esFeriado ? int.Parse(parametroIvaFeriados) : int.Parse(parametroIvaDefault);

                foreach (FacProducto lproducto in lproductos)
                {
                    lproducto.Valor = Math.Round(lproducto.ValorDoncho / denom, 2);
                    lproducto.CodigoIva = idCatDetalleIva;
                    unitOfWork.FacProductoR.Update(lproducto);
                }
                var productos = unitOfWork.FacProductoR.GetAllDto();
                cache.SetPermanent(Constantes.PRODUCTOS_ALL, productos);
                cache.SetPermanent(Constantes.ES_FERIADO, esFeriado);
                cache.SetPermanent(Constantes.ID_CATDETALLE_IVA, esFeriado ? CdIvaFeriados.Id : CdIvaDefault.Id);
                cache.SetPermanent(Constantes.PORCENTAJE_IVA, esFeriado ? porcentajeIvaFeriado : porcentajeIvaDefault);
                cache.SetPermanent(Constantes.CODIGO_IVA, esFeriado ? CdIvaFeriados.Valor : CdIvaDefault.Valor);
                cache.GetOrCreatePermanent(Constantes.JSON_SCHEMA_FACTURA, () => unitOfWork.GenParametroR.GetById(Constantes.JSON_SCHEMA_FACTURA));
                cache.GetOrCreatePermanent(Constantes.PATH_LOCAL_FACTURAS, () => unitOfWork.GenParametroR.GetById(Constantes.PATH_LOCAL_FACTURAS));
                cache.GetOrCreatePermanent(Constantes.CELINFOTRIBUTARIA, () => unitOfWork.CelInfoTributariaR.GetById(1));
                cache.GetOrCreatePermanent(Constantes.PATH_CERTIFICADO, () => unitOfWork.GenParametroR.GetById(Constantes.PATH_CERTIFICADO));
                cache.GetOrCreatePermanent(Constantes.PWD_CERTIFICADO, () => unitOfWork.GenParametroR.GetById(Constantes.PWD_CERTIFICADO));
                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
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
            if (string.IsNullOrWhiteSpace(config[clave]))
                errores.Add(clave);
        }
    }
}
