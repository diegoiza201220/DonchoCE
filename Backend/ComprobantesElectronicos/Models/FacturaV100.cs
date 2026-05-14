
// NOTA: El código generado puede requerir, como mínimo, .NET Framework 4.5 o .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class factura
{

    private facturaInfoTributaria infoTributariaField;

    private facturaInfoFactura infoFacturaField;

    private facturaDetalle[] detallesField;

    private facturaReembolsoDetalle[] reembolsosField;

    private facturaTipoNegociable tipoNegociableField;

    private facturaMaquinaFiscal maquinaFiscalField;

    private facturaCampoAdicional[] infoAdicionalField;

    private string idField;

    private string versionField;

    /// <remarks/>
    public facturaInfoTributaria infoTributaria
    {
        get
        {
            return this.infoTributariaField;
        }
        set
        {
            this.infoTributariaField = value;
        }
    }

    /// <remarks/>
    public facturaInfoFactura infoFactura
    {
        get
        {
            return this.infoFacturaField;
        }
        set
        {
            this.infoFacturaField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("detalle", IsNullable = false)]
    public facturaDetalle[] detalles
    {
        get
        {
            return this.detallesField;
        }
        set
        {
            this.detallesField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("reembolsoDetalle", IsNullable = false)]
    public facturaReembolsoDetalle[] reembolsos
    {
        get
        {
            return this.reembolsosField;
        }
        set
        {
            this.reembolsosField = value;
        }
    }

    /// <remarks/>
    public facturaTipoNegociable tipoNegociable
    {
        get
        {
            return this.tipoNegociableField;
        }
        set
        {
            this.tipoNegociableField = value;
        }
    }

    /// <remarks/>
    public facturaMaquinaFiscal maquinaFiscal
    {
        get
        {
            return this.maquinaFiscalField;
        }
        set
        {
            this.maquinaFiscalField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("campoAdicional", IsNullable = false)]
    public facturaCampoAdicional[] infoAdicional
    {
        get
        {
            return this.infoAdicionalField;
        }
        set
        {
            this.infoAdicionalField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string version
    {
        get
        {
            return this.versionField;
        }
        set
        {
            this.versionField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class facturaInfoTributaria
{

    private byte ambienteField;

    private byte tipoEmisionField;

    private string razonSocialField;

    private string nombreComercialField;

    private byte rucField;

    private byte claveAccesoField;

    private byte codDocField;

    private byte estabField;

    private byte ptoEmiField;

    private byte secuencialField;

    private string dirMatrizField;

    private byte agenteRetencionField;

    private string contribuyenteRimpeField;

    /// <remarks/>
    public byte ambiente
    {
        get
        {
            return this.ambienteField;
        }
        set
        {
            this.ambienteField = value;
        }
    }

    /// <remarks/>
    public byte tipoEmision
    {
        get
        {
            return this.tipoEmisionField;
        }
        set
        {
            this.tipoEmisionField = value;
        }
    }

    /// <remarks/>
    public string razonSocial
    {
        get
        {
            return this.razonSocialField;
        }
        set
        {
            this.razonSocialField = value;
        }
    }

    /// <remarks/>
    public string nombreComercial
    {
        get
        {
            return this.nombreComercialField;
        }
        set
        {
            this.nombreComercialField = value;
        }
    }

    /// <remarks/>
    public byte ruc
    {
        get
        {
            return this.rucField;
        }
        set
        {
            this.rucField = value;
        }
    }

    /// <remarks/>
    public byte claveAcceso
    {
        get
        {
            return this.claveAccesoField;
        }
        set
        {
            this.claveAccesoField = value;
        }
    }

    /// <remarks/>
    public byte codDoc
    {
        get
        {
            return this.codDocField;
        }
        set
        {
            this.codDocField = value;
        }
    }

    /// <remarks/>
    public byte estab
    {
        get
        {
            return this.estabField;
        }
        set
        {
            this.estabField = value;
        }
    }

    /// <remarks/>
    public byte ptoEmi
    {
        get
        {
            return this.ptoEmiField;
        }
        set
        {
            this.ptoEmiField = value;
        }
    }

    /// <remarks/>
    public byte secuencial
    {
        get
        {
            return this.secuencialField;
        }
        set
        {
            this.secuencialField = value;
        }
    }

    /// <remarks/>
    public string dirMatriz
    {
        get
        {
            return this.dirMatrizField;
        }
        set
        {
            this.dirMatrizField = value;
        }
    }

    /// <remarks/>
    public byte agenteRetencion
    {
        get
        {
            return this.agenteRetencionField;
        }
        set
        {
            this.agenteRetencionField = value;
        }
    }

    /// <remarks/>
    public string contribuyenteRimpe
    {
        get
        {
            return this.contribuyenteRimpeField;
        }
        set
        {
            this.contribuyenteRimpeField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class facturaInfoFactura
{

    private string fechaEmisionField;

    private string dirEstablecimientoField;

    private string contribuyenteEspecialField;

    private string obligadoContabilidadField;

    private string comercioExteriorField;

    private string incoTermFacturaField;

    private string lugarIncoTermField;

    private byte paisOrigenField;

    private string puertoEmbarqueField;

    private string puertoDestinoField;

    private byte paisDestinoField;

    private byte paisAdquisicionField;

    private byte tipoIdentificacionCompradorField;

    private string guiaRemisionField;

    private string razonSocialCompradorField;

    private string identificacionCompradorField;

    private string direccionCompradorField;

    private decimal totalSinImpuestosField;

    private decimal totalSubsidioField;

    private string incoTermTotalSinImpuestosField;

    private decimal totalDescuentoField;

    private byte codDocReembolsoField;

    private decimal totalComprobantesReembolsoField;

    private decimal totalBaseImponibleReembolsoField;

    private decimal totalImpuestoReembolsoField;

    private facturaInfoFacturaTotalImpuesto[] totalConImpuestosField;

    private facturaInfoFacturaCompensacion[] compensacionesField;

    private decimal propinaField;

    private decimal fleteInternacionalField;

    private decimal seguroInternacionalField;

    private decimal gastosAduanerosField;

    private decimal gastosTransporteOtrosField;

    private decimal importeTotalField;

    private string monedaField;

    private string placaField;

    private facturaInfoFacturaPago[] pagosField;

    private decimal valorRetIvaField;

    private decimal valorRetRentaField;

    /// <remarks/>
    public string fechaEmision
    {
        get
        {
            return this.fechaEmisionField;
        }
        set
        {
            this.fechaEmisionField = value;
        }
    }

    /// <remarks/>
    public string dirEstablecimiento
    {
        get
        {
            return this.dirEstablecimientoField;
        }
        set
        {
            this.dirEstablecimientoField = value;
        }
    }

    /// <remarks/>
    public string contribuyenteEspecial
    {
        get
        {
            return this.contribuyenteEspecialField;
        }
        set
        {
            this.contribuyenteEspecialField = value;
        }
    }

    /// <remarks/>
    public string obligadoContabilidad
    {
        get
        {
            return this.obligadoContabilidadField;
        }
        set
        {
            this.obligadoContabilidadField = value;
        }
    }

    /// <remarks/>
    public string comercioExterior
    {
        get
        {
            return this.comercioExteriorField;
        }
        set
        {
            this.comercioExteriorField = value;
        }
    }

    /// <remarks/>
    public string incoTermFactura
    {
        get
        {
            return this.incoTermFacturaField;
        }
        set
        {
            this.incoTermFacturaField = value;
        }
    }

    /// <remarks/>
    public string lugarIncoTerm
    {
        get
        {
            return this.lugarIncoTermField;
        }
        set
        {
            this.lugarIncoTermField = value;
        }
    }

    /// <remarks/>
    public byte paisOrigen
    {
        get
        {
            return this.paisOrigenField;
        }
        set
        {
            this.paisOrigenField = value;
        }
    }

    /// <remarks/>
    public string puertoEmbarque
    {
        get
        {
            return this.puertoEmbarqueField;
        }
        set
        {
            this.puertoEmbarqueField = value;
        }
    }

    /// <remarks/>
    public string puertoDestino
    {
        get
        {
            return this.puertoDestinoField;
        }
        set
        {
            this.puertoDestinoField = value;
        }
    }

    /// <remarks/>
    public byte paisDestino
    {
        get
        {
            return this.paisDestinoField;
        }
        set
        {
            this.paisDestinoField = value;
        }
    }

    /// <remarks/>
    public byte paisAdquisicion
    {
        get
        {
            return this.paisAdquisicionField;
        }
        set
        {
            this.paisAdquisicionField = value;
        }
    }

    /// <remarks/>
    public byte tipoIdentificacionComprador
    {
        get
        {
            return this.tipoIdentificacionCompradorField;
        }
        set
        {
            this.tipoIdentificacionCompradorField = value;
        }
    }

    /// <remarks/>
    public string guiaRemision
    {
        get
        {
            return this.guiaRemisionField;
        }
        set
        {
            this.guiaRemisionField = value;
        }
    }

    /// <remarks/>
    public string razonSocialComprador
    {
        get
        {
            return this.razonSocialCompradorField;
        }
        set
        {
            this.razonSocialCompradorField = value;
        }
    }

    /// <remarks/>
    public string identificacionComprador
    {
        get
        {
            return this.identificacionCompradorField;
        }
        set
        {
            this.identificacionCompradorField = value;
        }
    }

    /// <remarks/>
    public string direccionComprador
    {
        get
        {
            return this.direccionCompradorField;
        }
        set
        {
            this.direccionCompradorField = value;
        }
    }

    /// <remarks/>
    public decimal totalSinImpuestos
    {
        get
        {
            return this.totalSinImpuestosField;
        }
        set
        {
            this.totalSinImpuestosField = value;
        }
    }

    /// <remarks/>
    public decimal totalSubsidio
    {
        get
        {
            return this.totalSubsidioField;
        }
        set
        {
            this.totalSubsidioField = value;
        }
    }

    /// <remarks/>
    public string incoTermTotalSinImpuestos
    {
        get
        {
            return this.incoTermTotalSinImpuestosField;
        }
        set
        {
            this.incoTermTotalSinImpuestosField = value;
        }
    }

    /// <remarks/>
    public decimal totalDescuento
    {
        get
        {
            return this.totalDescuentoField;
        }
        set
        {
            this.totalDescuentoField = value;
        }
    }

    /// <remarks/>
    public byte codDocReembolso
    {
        get
        {
            return this.codDocReembolsoField;
        }
        set
        {
            this.codDocReembolsoField = value;
        }
    }

    /// <remarks/>
    public decimal totalComprobantesReembolso
    {
        get
        {
            return this.totalComprobantesReembolsoField;
        }
        set
        {
            this.totalComprobantesReembolsoField = value;
        }
    }

    /// <remarks/>
    public decimal totalBaseImponibleReembolso
    {
        get
        {
            return this.totalBaseImponibleReembolsoField;
        }
        set
        {
            this.totalBaseImponibleReembolsoField = value;
        }
    }

    /// <remarks/>
    public decimal totalImpuestoReembolso
    {
        get
        {
            return this.totalImpuestoReembolsoField;
        }
        set
        {
            this.totalImpuestoReembolsoField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("totalImpuesto", IsNullable = false)]
    public facturaInfoFacturaTotalImpuesto[] totalConImpuestos
    {
        get
        {
            return this.totalConImpuestosField;
        }
        set
        {
            this.totalConImpuestosField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("compensacion", IsNullable = false)]
    public facturaInfoFacturaCompensacion[] compensaciones
    {
        get
        {
            return this.compensacionesField;
        }
        set
        {
            this.compensacionesField = value;
        }
    }

    /// <remarks/>
    public decimal propina
    {
        get
        {
            return this.propinaField;
        }
        set
        {
            this.propinaField = value;
        }
    }

    /// <remarks/>
    public decimal fleteInternacional
    {
        get
        {
            return this.fleteInternacionalField;
        }
        set
        {
            this.fleteInternacionalField = value;
        }
    }

    /// <remarks/>
    public decimal seguroInternacional
    {
        get
        {
            return this.seguroInternacionalField;
        }
        set
        {
            this.seguroInternacionalField = value;
        }
    }

    /// <remarks/>
    public decimal gastosAduaneros
    {
        get
        {
            return this.gastosAduanerosField;
        }
        set
        {
            this.gastosAduanerosField = value;
        }
    }

    /// <remarks/>
    public decimal gastosTransporteOtros
    {
        get
        {
            return this.gastosTransporteOtrosField;
        }
        set
        {
            this.gastosTransporteOtrosField = value;
        }
    }

    /// <remarks/>
    public decimal importeTotal
    {
        get
        {
            return this.importeTotalField;
        }
        set
        {
            this.importeTotalField = value;
        }
    }

    /// <remarks/>
    public string moneda
    {
        get
        {
            return this.monedaField;
        }
        set
        {
            this.monedaField = value;
        }
    }

    /// <remarks/>
    public string placa
    {
        get
        {
            return this.placaField;
        }
        set
        {
            this.placaField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("pago", IsNullable = false)]
    public facturaInfoFacturaPago[] pagos
    {
        get
        {
            return this.pagosField;
        }
        set
        {
            this.pagosField = value;
        }
    }

    /// <remarks/>
    public decimal valorRetIva
    {
        get
        {
            return this.valorRetIvaField;
        }
        set
        {
            this.valorRetIvaField = value;
        }
    }

    /// <remarks/>
    public decimal valorRetRenta
    {
        get
        {
            return this.valorRetRentaField;
        }
        set
        {
            this.valorRetRentaField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class facturaInfoFacturaTotalImpuesto
{

    private byte codigoField;

    private byte codigoPorcentajeField;

    private decimal descuentoAdicionalField;

    private decimal baseImponibleField;

    private decimal tarifaField;

    private decimal valorField;

    private decimal valorDevolucionIvaField;

    /// <remarks/>
    public byte codigo
    {
        get
        {
            return this.codigoField;
        }
        set
        {
            this.codigoField = value;
        }
    }

    /// <remarks/>
    public byte codigoPorcentaje
    {
        get
        {
            return this.codigoPorcentajeField;
        }
        set
        {
            this.codigoPorcentajeField = value;
        }
    }

    /// <remarks/>
    public decimal descuentoAdicional
    {
        get
        {
            return this.descuentoAdicionalField;
        }
        set
        {
            this.descuentoAdicionalField = value;
        }
    }

    /// <remarks/>
    public decimal baseImponible
    {
        get
        {
            return this.baseImponibleField;
        }
        set
        {
            this.baseImponibleField = value;
        }
    }

    /// <remarks/>
    public decimal tarifa
    {
        get
        {
            return this.tarifaField;
        }
        set
        {
            this.tarifaField = value;
        }
    }

    /// <remarks/>
    public decimal valor
    {
        get
        {
            return this.valorField;
        }
        set
        {
            this.valorField = value;
        }
    }

    /// <remarks/>
    public decimal valorDevolucionIva
    {
        get
        {
            return this.valorDevolucionIvaField;
        }
        set
        {
            this.valorDevolucionIvaField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class facturaInfoFacturaCompensacion
{

    private byte codigoField;

    private decimal tarifaField;

    private decimal valorField;

    /// <remarks/>
    public byte codigo
    {
        get
        {
            return this.codigoField;
        }
        set
        {
            this.codigoField = value;
        }
    }

    /// <remarks/>
    public decimal tarifa
    {
        get
        {
            return this.tarifaField;
        }
        set
        {
            this.tarifaField = value;
        }
    }

    /// <remarks/>
    public decimal valor
    {
        get
        {
            return this.valorField;
        }
        set
        {
            this.valorField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class facturaInfoFacturaPago
{

    private byte formaPagoField;

    private decimal totalField;

    private decimal plazoField;

    private string unidadTiempoField;

    /// <remarks/>
    public byte formaPago
    {
        get
        {
            return this.formaPagoField;
        }
        set
        {
            this.formaPagoField = value;
        }
    }

    /// <remarks/>
    public decimal total
    {
        get
        {
            return this.totalField;
        }
        set
        {
            this.totalField = value;
        }
    }

    /// <remarks/>
    public decimal plazo
    {
        get
        {
            return this.plazoField;
        }
        set
        {
            this.plazoField = value;
        }
    }

    /// <remarks/>
    public string unidadTiempo
    {
        get
        {
            return this.unidadTiempoField;
        }
        set
        {
            this.unidadTiempoField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class facturaDetalle
{

    private string codigoPrincipalField;

    private string codigoAuxiliarField;

    private string descripcionField;

    private string unidadMedidaField;

    private decimal cantidadField;

    private decimal precioUnitarioField;

    private decimal precioSinSubsidioField;

    private decimal descuentoField;

    private decimal precioTotalSinImpuestoField;

    private facturaDetalleDetAdicional[] detallesAdicionalesField;

    private facturaDetalleImpuesto[] impuestosField;

    /// <remarks/>
    public string codigoPrincipal
    {
        get
        {
            return this.codigoPrincipalField;
        }
        set
        {
            this.codigoPrincipalField = value;
        }
    }

    /// <remarks/>
    public string codigoAuxiliar
    {
        get
        {
            return this.codigoAuxiliarField;
        }
        set
        {
            this.codigoAuxiliarField = value;
        }
    }

    /// <remarks/>
    public string descripcion
    {
        get
        {
            return this.descripcionField;
        }
        set
        {
            this.descripcionField = value;
        }
    }

    /// <remarks/>
    public string unidadMedida
    {
        get
        {
            return this.unidadMedidaField;
        }
        set
        {
            this.unidadMedidaField = value;
        }
    }

    /// <remarks/>
    public decimal cantidad
    {
        get
        {
            return this.cantidadField;
        }
        set
        {
            this.cantidadField = value;
        }
    }

    /// <remarks/>
    public decimal precioUnitario
    {
        get
        {
            return this.precioUnitarioField;
        }
        set
        {
            this.precioUnitarioField = value;
        }
    }

    /// <remarks/>
    public decimal precioSinSubsidio
    {
        get
        {
            return this.precioSinSubsidioField;
        }
        set
        {
            this.precioSinSubsidioField = value;
        }
    }

    /// <remarks/>
    public decimal descuento
    {
        get
        {
            return this.descuentoField;
        }
        set
        {
            this.descuentoField = value;
        }
    }

    /// <remarks/>
    public decimal precioTotalSinImpuesto
    {
        get
        {
            return this.precioTotalSinImpuestoField;
        }
        set
        {
            this.precioTotalSinImpuestoField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("detAdicional", IsNullable = false)]
    public facturaDetalleDetAdicional[] detallesAdicionales
    {
        get
        {
            return this.detallesAdicionalesField;
        }
        set
        {
            this.detallesAdicionalesField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("impuesto", IsNullable = false)]
    public facturaDetalleImpuesto[] impuestos
    {
        get
        {
            return this.impuestosField;
        }
        set
        {
            this.impuestosField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class facturaDetalleDetAdicional
{

    private string nombreField;

    private string valorField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string nombre
    {
        get
        {
            return this.nombreField;
        }
        set
        {
            this.nombreField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string valor
    {
        get
        {
            return this.valorField;
        }
        set
        {
            this.valorField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class facturaDetalleImpuesto
{

    private byte codigoField;

    private byte codigoPorcentajeField;

    private decimal tarifaField;

    private decimal baseImponibleField;

    private decimal valorField;

    /// <remarks/>
    public byte codigo
    {
        get
        {
            return this.codigoField;
        }
        set
        {
            this.codigoField = value;
        }
    }

    /// <remarks/>
    public byte codigoPorcentaje
    {
        get
        {
            return this.codigoPorcentajeField;
        }
        set
        {
            this.codigoPorcentajeField = value;
        }
    }

    /// <remarks/>
    public decimal tarifa
    {
        get
        {
            return this.tarifaField;
        }
        set
        {
            this.tarifaField = value;
        }
    }

    /// <remarks/>
    public decimal baseImponible
    {
        get
        {
            return this.baseImponibleField;
        }
        set
        {
            this.baseImponibleField = value;
        }
    }

    /// <remarks/>
    public decimal valor
    {
        get
        {
            return this.valorField;
        }
        set
        {
            this.valorField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class facturaReembolsoDetalle
{

    private byte tipoIdentificacionProveedorReembolsoField;

    private string identificacionProveedorReembolsoField;

    private byte codPaisPagoProveedorReembolsoField;

    private byte tipoProveedorReembolsoField;

    private byte codDocReembolsoField;

    private byte estabDocReembolsoField;

    private byte ptoEmiDocReembolsoField;

    private byte secuencialDocReembolsoField;

    private string fechaEmisionDocReembolsoField;

    private byte numeroautorizacionDocReembField;

    private facturaReembolsoDetalleDetalleImpuesto[] detalleImpuestosField;

    private facturaReembolsoDetalleCompensacionReembolso[] compensacionesReembolsoField;

    /// <remarks/>
    public byte tipoIdentificacionProveedorReembolso
    {
        get
        {
            return this.tipoIdentificacionProveedorReembolsoField;
        }
        set
        {
            this.tipoIdentificacionProveedorReembolsoField = value;
        }
    }

    /// <remarks/>
    public string identificacionProveedorReembolso
    {
        get
        {
            return this.identificacionProveedorReembolsoField;
        }
        set
        {
            this.identificacionProveedorReembolsoField = value;
        }
    }

    /// <remarks/>
    public byte codPaisPagoProveedorReembolso
    {
        get
        {
            return this.codPaisPagoProveedorReembolsoField;
        }
        set
        {
            this.codPaisPagoProveedorReembolsoField = value;
        }
    }

    /// <remarks/>
    public byte tipoProveedorReembolso
    {
        get
        {
            return this.tipoProveedorReembolsoField;
        }
        set
        {
            this.tipoProveedorReembolsoField = value;
        }
    }

    /// <remarks/>
    public byte codDocReembolso
    {
        get
        {
            return this.codDocReembolsoField;
        }
        set
        {
            this.codDocReembolsoField = value;
        }
    }

    /// <remarks/>
    public byte estabDocReembolso
    {
        get
        {
            return this.estabDocReembolsoField;
        }
        set
        {
            this.estabDocReembolsoField = value;
        }
    }

    /// <remarks/>
    public byte ptoEmiDocReembolso
    {
        get
        {
            return this.ptoEmiDocReembolsoField;
        }
        set
        {
            this.ptoEmiDocReembolsoField = value;
        }
    }

    /// <remarks/>
    public byte secuencialDocReembolso
    {
        get
        {
            return this.secuencialDocReembolsoField;
        }
        set
        {
            this.secuencialDocReembolsoField = value;
        }
    }

    /// <remarks/>
    public string fechaEmisionDocReembolso
    {
        get
        {
            return this.fechaEmisionDocReembolsoField;
        }
        set
        {
            this.fechaEmisionDocReembolsoField = value;
        }
    }

    /// <remarks/>
    public byte numeroautorizacionDocReemb
    {
        get
        {
            return this.numeroautorizacionDocReembField;
        }
        set
        {
            this.numeroautorizacionDocReembField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("detalleImpuesto", IsNullable = false)]
    public facturaReembolsoDetalleDetalleImpuesto[] detalleImpuestos
    {
        get
        {
            return this.detalleImpuestosField;
        }
        set
        {
            this.detalleImpuestosField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("compensacionReembolso", IsNullable = false)]
    public facturaReembolsoDetalleCompensacionReembolso[] compensacionesReembolso
    {
        get
        {
            return this.compensacionesReembolsoField;
        }
        set
        {
            this.compensacionesReembolsoField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class facturaReembolsoDetalleDetalleImpuesto
{

    private byte codigoField;

    private byte codigoPorcentajeField;

    private decimal tarifaField;

    private decimal baseImponibleReembolsoField;

    private decimal impuestoReembolsoField;

    /// <remarks/>
    public byte codigo
    {
        get
        {
            return this.codigoField;
        }
        set
        {
            this.codigoField = value;
        }
    }

    /// <remarks/>
    public byte codigoPorcentaje
    {
        get
        {
            return this.codigoPorcentajeField;
        }
        set
        {
            this.codigoPorcentajeField = value;
        }
    }

    /// <remarks/>
    public decimal tarifa
    {
        get
        {
            return this.tarifaField;
        }
        set
        {
            this.tarifaField = value;
        }
    }

    /// <remarks/>
    public decimal baseImponibleReembolso
    {
        get
        {
            return this.baseImponibleReembolsoField;
        }
        set
        {
            this.baseImponibleReembolsoField = value;
        }
    }

    /// <remarks/>
    public decimal impuestoReembolso
    {
        get
        {
            return this.impuestoReembolsoField;
        }
        set
        {
            this.impuestoReembolsoField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class facturaReembolsoDetalleCompensacionReembolso
{

    private byte codigoField;

    private decimal tarifaField;

    private decimal valorField;

    /// <remarks/>
    public byte codigo
    {
        get
        {
            return this.codigoField;
        }
        set
        {
            this.codigoField = value;
        }
    }

    /// <remarks/>
    public decimal tarifa
    {
        get
        {
            return this.tarifaField;
        }
        set
        {
            this.tarifaField = value;
        }
    }

    /// <remarks/>
    public decimal valor
    {
        get
        {
            return this.valorField;
        }
        set
        {
            this.valorField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class facturaTipoNegociable
{

    private string correoField;

    /// <remarks/>
    public string correo
    {
        get
        {
            return this.correoField;
        }
        set
        {
            this.correoField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class facturaMaquinaFiscal
{

    private string marcaField;

    private string modeloField;

    private string serieField;

    /// <remarks/>
    public string marca
    {
        get
        {
            return this.marcaField;
        }
        set
        {
            this.marcaField = value;
        }
    }

    /// <remarks/>
    public string modelo
    {
        get
        {
            return this.modeloField;
        }
        set
        {
            this.modeloField = value;
        }
    }

    /// <remarks/>
    public string serie
    {
        get
        {
            return this.serieField;
        }
        set
        {
            this.serieField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class facturaCampoAdicional
{

    private string nombreField;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string nombre
    {
        get
        {
            return this.nombreField;
        }
        set
        {
            this.nombreField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

