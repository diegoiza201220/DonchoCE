using EFModel.DTO;

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

        public static notaCredito ObtenerNotaCredito(FacOrdenDTO orden)
        {
            notaCredito notaCredito = new notaCredito()
            {
                id = "comprobante",
                version = "1.0.0",
                infoTributaria = new notaCreditoInfoTributaria()
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
                    secuencial = orden.NumeroFactura,
                },
                infoNotaCredito = new notaCreditoInfoNotaCredito()
                {
                    fechaEmision = orden.Fecha.ToLocalTime().ToString("dd/MM/yyyy"),
                    dirEstablecimiento = orden.DireccionEstablecimiento,
                    obligadoContabilidad = orden.ObligadoContabilidad,
                    tipoIdentificacionComprador = "05",
                    razonSocialComprador = $"{orden.Cliente.Apellido} {orden.Cliente.Nombre}",
                    identificacionComprador = orden.Cliente.CedulaRuc,
                    //direccionComprador = orden.Cliente.Direccion ?? "SIN DIRECCION",
                    totalSinImpuestos = orden.TotalSinImpuestos,
                    //totalDescuento = 0,
                    //propina = 0,
                    //importeTotal = orden.TotalOrden,
                    moneda = "DOLAR",
                    codDocModificado = orden.CodDoc,
                    contribuyenteEspecial = "NO",
                    fechaEmisionDocSustento = orden.Fecha.ToLocalTime().ToString("dd/MM/yyyy"),
                    motivo = "DEVOLUCION",
                    numDocModificado = $"{orden.Establecimiento}-{orden.PuntoEmision}-{orden.NumeroFactura}",
                    valorModificacion = orden.TotalOrden,
                    totalConImpuestos =
                    [
                        new() {
                            codigo = orden.ImpuestoCodigo,
                            codigoPorcentaje = orden.ImpuestoCodigoPorcentaje,
                            baseImponible = orden.ImpuestoBaseImponible,
                            valor = orden.ImpuestoValor,
                            valorDevolucionIva = 0
                        }
                    ]
                }
            };

            List<notaCreditoDetalle> lncdetalle = [];
            foreach (var item in orden.FacDetalleOrdens)
            {
                lncdetalle.Add(new notaCreditoDetalle
                {
                    codigoInterno = item.ProductoId.ToString(),
                    descripcion = item.Nombre ?? "ALIMENTACION",
                    cantidad = item.Cantidad,
                    precioUnitario = item.PrecioUnitario,
                    descuento = 0,
                    precioTotalSinImpuesto = item.PrecioTotal,
                    codigoAdicional = item.ProductoId.ToString(),
                    impuestos =
                    [
                            new()
                            {
                                codigo           = 2, // codigo IVA
                                codigoPorcentaje = item.ImpuestoCodigoPorcentaje ,   // IVA 15% Ecuador
                                tarifa           = item.ImpuestoTarifa,
                                baseImponible    = item.PrecioTotal, //item.PrecioTotal,
                                valor            = item.ImpuestoValor //item.ImpuestoValor
                            }
                    ]
                });
            }
            notaCredito.detalles = [.. lncdetalle];
            return notaCredito;
        }
    }
}