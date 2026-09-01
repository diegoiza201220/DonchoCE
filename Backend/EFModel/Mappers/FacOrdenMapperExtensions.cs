using EFModel.DTO;
using EFModel.Models;

namespace EFModel.Mappers
{
    public static class FacOrdenMapperExtensions
    {
        public static FacOrden FromDTO(this FacOrdenDTO orden)
        {
            return new FacOrden
            {
                Clienteid = orden.Clienteid,
                Sucursalid = orden.Sucursalid,
                FechaInteger = orden.FechaInteger,
                Secuencial = orden.Secuencial,
                Fecha = DateTime.Now.ToUniversalTime(),
                TipoPago = orden.TipoPago,
                TotalOrden = orden.TotalOrden,
                ImpuestoCodigo = orden.ImpuestoCodigo,
                ImpuestoCodigoPorcentaje = orden.ImpuestoCodigoPorcentaje,
                ImpuestoBaseImponible = orden.ImpuestoBaseImponible,
                ImpuestoValor = orden.ImpuestoValor,
                ImpuestoPorcentaje = orden.ImpuestoPorcentaje,
                TotalSinImpuestos = orden.TotalSinImpuestos,
                UsuarioRegistro = orden.UsuarioRegistro,
                EsFactura = orden.EsFactura,
                DocumentoPago = orden.DocumentoPago,
                EsNotaCredito = orden.EsNotaCredito,
                NotaCreditoClaveNumeroAutorizacion = orden.NotaCreditoClaveNumeroAutorizacion,
                NotaCreditoNumeroNotaCredito = orden.NotaCreditoNumeroNotaCredito,
                NotaCreditoMotivo = orden.NotaCreditoMotivo,
                NotaCreditoFecha = orden.NotaCreditoFecha,
                FacDetalleOrdens = [.. orden.FacDetalleOrdens.Select(d => new FacDetalleOrden
                {
                    Cantidad = d.Cantidad,
                    ImpuestoCodigo = d.ImpuestoCodigo,
                    Ordenid = d.Ordenid,
                    PedidoACocina = d.PedidoACocina,
                    PrecioTotal = d.PrecioTotal,
                    PrecioUnitario = d.PrecioUnitario,
                    Productoid = d.ProductoId,
                    ImpuestoCodigoPorcentaje = d.ImpuestoCodigoPorcentaje,
                    ImpuestoTarifa = d.ImpuestoTarifa,
                    ImpuestoValor = d.ImpuestoValor
                })]
            };
        }
    }
}
