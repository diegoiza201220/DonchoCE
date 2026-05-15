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
        private readonly ComprobanteService _comprobanteService;
        public OrdenService(IUnitOfWork uow, ComprobanteService comprobanteService)
        {
            _uow = uow;
            _comprobanteService = comprobanteService;
        }
        public async Task<FacOrden> FacturarAsync(FacOrdenDTO orden)
        {
            var secuencia = await _uow.FacSecuenciaDiaR.GetSecuenciaAsync();
            if (secuencia is null)
                throw new InvalidOperationException("No existe registro de secuencia del día.");

            orden.Clienteid = 4; // Temporal, luego se obtiene del cliente registrado en el sistema o se crea uno nuevo

            var facOrden = new FacOrden
            {
                Clienteid = orden.Clienteid,
                FechaInteger = orden.FechaInteger,
                Secuencial = secuencia.Secuencia,
                Fecha = orden.Fecha,
                TipoPago = orden.TipoPago,
                TotalOrden = orden.TotalOrden,
                ImpuestoCodigo = orden.ImpuestoCodigo,
                ImpuestoCodigoPorcentaje = orden.ImpuestoCodigoPorcentaje,
                ImpuestoBaseImponible = orden.ImpuestoBaseImponible,
                ImpuestoValor = orden.ImpuestoValor,
                TotalSinImpuestos = orden.TotalSinImpuestos,
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
            
            FacCliente facCliente = await _uow.FacClienteR.GetByIdAsync(orden.Clienteid) ?? throw new InvalidOperationException("Cliente no encontrado.");
            facOrden.Cliente = facCliente;
            await _uow.SaveChangesAsync();

            
            //var xmlPlano = GenerarXmlFactura(orden!);   // tu método que arma el XML
            _ = _comprobanteService.EmitirFacturaAsync(facOrden);
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
