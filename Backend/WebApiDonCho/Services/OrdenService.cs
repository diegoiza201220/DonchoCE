using ComprobantesElectronicos.DTO.Sri;
using ComprobantesElectronicos.Services;
using EFModel.DTO;
using EFModel.DTO.Reportes;
using EFModel.DTO.Request;
using EFModel.Interfaces;
using EFModel.Mappers;
using EFModel.Models;
using EnvioCorreos.Models;
using EnvioCorreos.Services;
using WebApiDonCho.Helpers.ComprobantesElectronicos;

namespace WebApiDonCho.Services
{
    public class OrdenService
    {
        private readonly IUnitOfWork _uow;
        private readonly ComprobanteService _comprobanteService;
        private readonly EmailService _emailService;
        public OrdenService(IUnitOfWork uow, ComprobanteService comprobanteService, EmailService emailService)
        {
            _uow = uow;
            _comprobanteService = comprobanteService;
            _emailService = emailService;
        }
        public async Task<FacOrden> FacturarAsync(FacOrdenDTO orden)
        {
            var secuencia = await _uow.FacSecuenciaDiaR.GetSecuenciaAsync();
            if (secuencia is null)
                throw new InvalidOperationException("No existe registro de secuencia del día.");

            var codigoPorcentaje = Convert.ToInt16(_uow.GenCatalogoDetalleR.GetById(orden.ImpuestoCodigoPorcentaje).Valor);
            orden.ImpuestoCodigoPorcentaje = codigoPorcentaje;
            orden.FacDetalleOrdens.ForEach(d => d.ImpuestoCodigoPorcentaje = codigoPorcentaje);
            var facOrden = orden.ToDTO();

            if (orden.EsFactura)
            {
                orden.CodDoc = "01";
                CelSecuenciaSri celSecuenciaSri = _uow.CelSecuenciasSriR.GetByTipoDocumento("01");
                CelInfoTributaria celInfoTributaria = _uow.CelInfoTributariaR.GetById(1);
                InfoTributariaHelper.SetInformacion(orden, facOrden, celInfoTributaria, celSecuenciaSri, esProduccion: false);
                CelLogDocumento celLogDocumento = CelLogDocumentoHelper.CrearLogInicial(orden);
                await _uow.CelLogDocumentoR.AddAsync(celLogDocumento);
                celSecuenciaSri.SecuenciaActual++;
                _uow.CelSecuenciasSriR.Update(celSecuenciaSri);
            }   

            ActualizarSecuenciaOrdenesDiaria(secuencia, orden.FechaInteger);

            _uow.FacSecuenciaDiaR.Update(secuencia);
            await _uow.FacOrdenR.AddAsync(facOrden);
            await _uow.SaveChangesAsync();

            if (orden.EsFactura)
            {
                //ResultadoEmisionDTO resultado = await _comprobanteService.EmitirFacturaAsync(orden);
                _comprobanteService.EmitirFacturaAsync(orden);
                EmailMessage emailMessage = new EmailMessage
                {
                    Asunto = "Factura de su compra",
                    Cuerpo = $"Estimado {orden.Cliente.Nombre}, adjunto encontrará la factura de su compra. Gracias por elegirnos.",
                    Destinatarios = new List<string> { orden.Cliente.Email?? "diegoiza@hotmail.com" },
                    EsHtml = false
                };
                _ = _emailService.EnviarAsync(emailMessage);
                Console.WriteLine($"-------------------000000000000000000000000salio del hiloooooooo0000000000000000000------------------");
            }
            return facOrden;
        }

        private static void ActualizarSecuenciaOrdenesDiaria(FacSecuenciaDia secuencia, int fecha)
        {
            if (secuencia.Fecha == fecha)
                secuencia.Secuencia++;
            else
            {
                secuencia.Fecha = fecha;
                secuencia.Secuencia = 2;
            }
        }

        public async Task<IEnumerable<RptOrdenesPorFechasDTO>> GetOrdenesPorFechaAsync(RqOrdenesPorFechas rq)
        {
            var ordenes = await _uow.FacOrdenR.GetByFechas(rq.FechaIni, rq.FechaFin);

            return ordenes.Select(o => new RptOrdenesPorFechasDTO
            {
                ClienteId = o.Clienteid,
                Nombre = o.Cliente.Nombre,
                Apellido = o.Cliente.Apellido,
                FechaInteger = o.FechaInteger,
                Secuencial = o.Secuencial,
                Fecha = o.Fecha,
                TipoPago = o.TipoPago,
                TotalOrden = o.TotalOrden,
                UsuarioRegistro = o.UsuarioRegistro,
                EsFactura = o.EsFactura,
                NumeroFactura = o.NumeroFactura,
                DocumentoPago = o.DocumentoPago ?? ""
            });
        }

        public async Task<IEnumerable<RptProductosVendidosPorFechasDTO>> GetProductosVendidosPorFechaAsync(RqOrdenesPorFechas rq)
        {
            return await _uow.FacDetalleOrdenR.GetByFechasProductosVendidos(rq.FechaIni, rq.FechaFin);
        }
    }
}
