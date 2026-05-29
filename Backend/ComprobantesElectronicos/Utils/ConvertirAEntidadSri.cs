using EFModel.DTO;
using EFModel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComprobantesElectronicos.Utils
{
    public class ConvertirAEntidadSri
    {
        public static factura ObtenerFactura(FacOrdenDTO orden)
        {
            
            factura factura = new()
            {
                id = "comprobante",
                version = "1.0.0",
                infoTributaria = new facturaInfoTributaria()
                {
                    ambiente = 1,
                    tipoEmision = 1,
                    razonSocial = orden.RazonSocial,
                    nombreComercial = orden.NombreComercial,
                    ruc = orden.RucDonCho,
                    claveAcceso = orden.ClaveNumeroAutorizacion,
                    codDoc = orden.CodDoc,
                    contribuyenteRimpe = orden.ContibuyenteRimpe,
                    dirMatriz = orden.Direccionmatriz,
                    estab = orden.Establecimiento,
                    ptoEmi = orden.PuntoEmision,
                    secuencial = orden.NumeroFactura
                },
                infoFactura = new facturaInfoFactura()
                {
                    fechaEmision = orden.Fecha.ToLocalTime().ToString("dd/MM/yyyy"),
                    dirEstablecimiento = orden.DireccionEstablecimiento,
                    obligadoContabilidad = orden.ObligadoContabilidad,
                    tipoIdentificacionComprador = "05",
                    razonSocialComprador = $"{orden.Cliente.Apellido} {orden.Cliente.Nombre}",
                    identificacionComprador = orden.Cliente.CedulaRuc,
                    direccionComprador = orden.Cliente.Direccion ?? "SIN DIRECCION",
                    totalSinImpuestos = orden.TotalSinImpuestos,
                    totalDescuento = 0,
                    propina = 0,
                    importeTotal = orden.TotalOrden,
                    moneda = "DOLAR",
                }
            };

            factura.infoFactura.totalConImpuestos = new facturaInfoFacturaTotalImpuesto[]
            {
                new()
                {
                    codigo             = orden.ImpuestoCodigo,
                    codigoPorcentaje   = orden.ImpuestoCodigoPorcentaje,
                    descuentoAdicional = 0,
                    baseImponible      = orden.ImpuestoBaseImponible,
                    valor              = orden.ImpuestoValor,
                }
            };

            factura.infoFactura.pagos = new facturaInfoFacturaPago[]
            {
                new()
                {
                    formaPago   = orden.TipoPago == "EF"?"01":"20",
                    total       = orden.TotalOrden,
                    plazo       = 0,
                    unidadTiempo = "dias"
                }
            };

            List<facturaDetalle> lfdetalle = [];
            foreach (var item in orden.FacDetalleOrdens)
            {
                lfdetalle.Add(new facturaDetalle
                {
                    codigoPrincipal = item.ProductoId.ToString(),
                    descripcion = item.Nombre ?? "ALIMENTACION",
                    cantidad = item.Cantidad,
                    precioUnitario = item.PrecioUnitario,
                    descuento = 0,
                    precioTotalSinImpuesto = item.PrecioTotal,
                    impuestos = new facturaDetalleImpuesto[]
                    {
                        new()
                        {
                            codigo           = 2, // codigo IVA
                            codigoPorcentaje = item.ImpuestoCodigoPorcentaje ,   // IVA 15% Ecuador
                            tarifa           = item.ImpuestoTarifa,
                            baseImponible    = item.PrecioTotal, //item.PrecioTotal,
                            valor            = item.ImpuestoValor //item.ImpuestoValor
                        }
                    }
                });
            }
            factura.detalles = lfdetalle.ToArray();
            return factura;
        }
    }
}
