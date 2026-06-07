using ComprobantesElectronicos.DTO.Sri;
using ComprobantesElectronicos.Services;
using EFModel.DTO;
using EFModel.DTO.Reportes;
using EFModel.DTO.Request;
using EFModel.Interfaces;
using EFModel.Mappers;
using EFModel.Models;
using WebApiDonCho.Helpers.ComprobantesElectronicos;

namespace WebApiDonCho.Services
{
    public class OrdenService(IUnitOfWork uow, ComprobanteService comprobanteService)
    {
        public async Task<FacOrden> FacturarAsync(FacOrdenDTO orden)
        {
            var secuencia = await uow.FacSecuenciaDiaR.GetSecuenciaAsync() ?? throw new InvalidOperationException("No existe registro de secuencia del día.");

            var codigoPorcentaje = Convert.ToInt16(uow.GenCatalogoDetalleR.GetById(orden.ImpuestoCodigoPorcentaje).Valor);
            orden.ImpuestoCodigoPorcentaje = codigoPorcentaje;
            orden.FacDetalleOrdens.ForEach(d => d.ImpuestoCodigoPorcentaje = codigoPorcentaje);
            var facOrden = orden.ToDTO();

            ActualizarSecuenciaOrdenesDiaria(secuencia, orden.FechaInteger);

            uow.FacSecuenciaDiaR.Update(secuencia);
            await uow.FacOrdenR.AddAsync(facOrden);
            await uow.SaveChangesAsync();

            if (!orden.EsFactura) //retornamos desde aquí si no es factura, para evitar el proceso de emisión electrónica y envío de correo
            {
                return facOrden;
            }

            orden.CodDoc = "01";
            CelSecuenciaSri celSecuenciaSri = uow.CelSecuenciasSriR.GetByTipoDocumento("01");
            CelInfoTributaria celInfoTributaria = uow.CelInfoTributariaR.GetById(1);
            InfoTributariaHelper.SetInformacion(orden, facOrden, celInfoTributaria, celSecuenciaSri, esProduccion: false);
            CelLogDocumento celLogDocumento = CelLogDocumentoHelper.CrearLogInicial(orden);
            await uow.CelLogDocumentoR.AddAsync(celLogDocumento);
            celSecuenciaSri.SecuenciaActual++;
            uow.CelSecuenciasSriR.Update(celSecuenciaSri);
            await uow.SaveChangesAsync();

            _ = await comprobanteService.EmitirFacturaAsync(orden, celLogDocumento);
            //await uow.SaveChangesAsync();


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
            var ordenes = await uow.FacOrdenR.GetByFechas(rq.FechaIni, rq.FechaFin);

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
            return await uow.FacDetalleOrdenR.GetByFechasProductosVendidos(rq.FechaIni, rq.FechaFin);
        }
    }
}
