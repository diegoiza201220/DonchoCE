using EFModel.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using EFModel.Models;

namespace EFModel.Mappers
{
    public static class FacOrdenMapperExtensions
    {
        public static FacOrden ToDTO(this FacOrdenDTO orden)
        {
            return new FacOrden
            {
                Clienteid = orden.Clienteid,
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
                FacDetalleOrdens = orden.FacDetalleOrdens.Select(d => new FacDetalleOrden
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
                }).ToList()
            };
        }
    }
}
