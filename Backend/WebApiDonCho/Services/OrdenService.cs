using ComprobantesElectronicos.Services;
using EFModel.DTO;
using EFModel.DTO.Reportes;
using EFModel.DTO.Request;
using EFModel.Interfaces;
using EFModel.Models;

namespace WebApiDonCho.Services
{
    public class OrdenService
    {
        private readonly IUnitOfWork _uow;
        private readonly FirmaElectronicaService _firmaElectronicaService;
        public OrdenService(IUnitOfWork uow, FirmaElectronicaService firmaElectronica)
        {
            _uow = uow;
            _firmaElectronicaService = firmaElectronica;
        }
        public async Task<FacOrden> FacturarAsync(FacOrdenDTO orden)
        {
            var secuencia = await _uow.FacSecuenciaDiaR.GetSecuenciaAsync();
            if (secuencia is null)
                throw new InvalidOperationException("No existe registro de secuencia del día.");

            var facOrden = new FacOrden
            {
                Clienteid = orden.Clienteid,
                FechaInteger = orden.FechaInteger,
                Secuencial = secuencia.Secuencia,
                Fecha = orden.Fecha,
                TipoPago = orden.TipoPago,
                TotalOrden = orden.TotalOrden,
                ValorIva = orden.ValorIva,
                CodigoIva = orden.CodigoIva ?? 0,
                UsuarioRegistro = orden.UsuarioRegistro,
                EsFactura = orden.EsFactura,
                NumeroFactura = orden.NumeroFactura,
                DocumentoPago = orden.DocumentoPago,
                FacDetalleOrdens = orden.FacDetalleOrdens.Select(d => new FacDetalleOrden
                {
                    Cantidad = d.Cantidad,
                    CodigoIva = d.CodigoIva,
                    Ordenid = d.Ordenid,
                    PedidoACocina = d.PedidoACocina,
                    PrecioTotal = d.PrecioTotal,
                    PrecioUnitario = d.PrecioUnitario,
                    Productoid = d.ProductoId,
                    ValorIva = d.ValorIva
                }).ToList()
            };

            ActualizarSecuencia(secuencia, orden.FechaInteger);

            _uow.FacSecuenciaDiaR.Update(secuencia);
            await _uow.FacOrdenR.AddAsync(facOrden);
            await _uow.SaveChangesAsync();

            
            //var xmlPlano = GenerarXmlFactura(orden!);   // tu método que arma el XML
            var xmlFirmado = _firmaElectronicaService.GenerarXMLFactura();

            return facOrden;
        }

        private static void ActualizarSecuencia(FacSecuenciaDia secuencia, int fecha)
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
                ValorIva = o.ValorIva,
                CodigoIva = o.CodigoIva ?? 0,
                UsuarioRegistro = o.UsuarioRegistro,
                EsFactura = o.EsFactura,
                NumeroFactura = o.NumeroFactura,
                DocumentoPago = o.DocumentoPago ?? ""
            });
        }

        public async Task<IEnumerable<RptProductosVendidosPorFechasDTO>> GetProductosVendidosPorFechaAsync(RqOrdenesPorFechas rq)
        {
            var detalles = await _uow.FacDetalleOrdenR.GetByFechasProductosVendidos(rq.FechaIni, rq.FechaFin);
            var catgroup = detalles.GroupBy(c => c.Producto.Nombre)
                .Select(g => new
                {
                    g.Key,
                    SUM = g.Sum(s => s.Cantidad)
                });
            return catgroup.OrderByDescending(o => o.SUM).Select(d => new RptProductosVendidosPorFechasDTO
            {
                Plato = d.Key,
                Cantidad = d.SUM
            });
        }
    }
}
