using EFModel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComprobantesElectronicos.Utils
{
    public class ConvertirAEntidadSri
    {
        public static string GenerarXMLPlano<T>(T entidad)
        {
            return XmlGenerator.ConvertirClaseAXml(entidad);
        }

        public static factura ObtenerFactura(FacOrden orden)
        {
            
            factura factura = new()
            {
                id = "comprobante",
                version = "1.0.0",
                infoTributaria = new facturaInfoTributaria()
                {
                    ambiente = 1,
                    tipoEmision = 1,
                    razonSocial = "IRENE PAZMIÑO",
                    nombreComercial = "IRENE PAZMIÑO",
                    ruc = "1714802681001",
                    claveAcceso = orden.ClaveNumeroAutorizacion,
                    codDoc = "01",
                    contribuyenteRimpe = "CONTRIBUYENTE RÉGIMEN RIMPE",
                    dirMatriz = "CENTRO HISTÓRICO QUITO",
                    estab = orden.Establecimiento,
                    ptoEmi = orden.PuntoEmision,
                    secuencial = orden.NumeroFactura
                },
                infoFactura = new facturaInfoFactura()
                {
                    fechaEmision = orden.Fecha.ToString("dd/MM/yyyy"),
                    dirEstablecimiento = "CENTRO HISTORICO",
                    obligadoContabilidad = "NO",
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
                    codigo             = 2, // orden.ImpuestoCodigo,
                    codigoPorcentaje   = 4, //orden.ImpuestoCodigoPorcentaje,
                    descuentoAdicional = 0,
                    baseImponible      = 1, //orden.ImpuestoBaseImponible,
                    valor              = 0.15m //orden.ImpuestoValor,
                }
            };

            factura.infoFactura.pagos = new facturaInfoFacturaPago[]
            {
                new()
                {
                    formaPago   = "01",
                    total       = 1.15m, //orden.TotalOrden,
                    plazo       = 0,
                    unidadTiempo = "dias"
                }
            };

            List<facturaDetalle> lfdetalle = [];
            foreach (var item in orden.FacDetalleOrdens)
            {
                lfdetalle.Add(new facturaDetalle
                {
                    codigoPrincipal = item.Productoid.ToString(),
                    descripcion = item.Producto?.Nombre ?? "ALIMENTACION",
                    cantidad = item.Cantidad,
                    precioUnitario = item.PrecioUnitario,
                    descuento = 0,
                    precioTotalSinImpuesto = item.PrecioTotal,
                    impuestos = new facturaDetalleImpuesto[]
                    {
                        new()
                        {
                            codigo           = 2,
                            codigoPorcentaje = 4,   // IVA 15% Ecuador
                            tarifa           = 15,
                            baseImponible    = 1, //item.PrecioTotal,
                            valor            = 0.15m //item.ImpuestoValor
                        }
                    }
                });
            }
            factura.detalles = lfdetalle.ToArray();
            return factura;
        }
    }
}
