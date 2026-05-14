
// NOTA: El código generado puede requerir, como mínimo, .NET Framework 4.5 o .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class notaCredito
{

    private notaCreditoInfoTributaria infoTributariaField;

    private notaCreditoInfoNotaCredito infoNotaCreditoField;

    private notaCreditoDetalle[] detallesField;

    private notaCreditoMaquinaFiscal maquinaFiscalField;

    private notaCreditoCampoAdicional[] infoAdicionalField;

    private string idField;

    private string versionField;

    /// <remarks/>
    public notaCreditoInfoTributaria infoTributaria
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
    public notaCreditoInfoNotaCredito infoNotaCredito
    {
        get
        {
            return this.infoNotaCreditoField;
        }
        set
        {
            this.infoNotaCreditoField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("detalle", IsNullable = false)]
    public notaCreditoDetalle[] detalles
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
    public notaCreditoMaquinaFiscal maquinaFiscal
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
    public notaCreditoCampoAdicional[] infoAdicional
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
public partial class notaCreditoInfoTributaria
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
public partial class notaCreditoInfoNotaCredito
{

    private string fechaEmisionField;

    private string dirEstablecimientoField;

    private byte tipoIdentificacionCompradorField;

    private string razonSocialCompradorField;

    private string identificacionCompradorField;

    private string contribuyenteEspecialField;

    private string obligadoContabilidadField;

    private string riseField;

    private byte codDocModificadoField;

    private string numDocModificadoField;

    private string fechaEmisionDocSustentoField;

    private decimal totalSinImpuestosField;

    private notaCreditoInfoNotaCreditoCompensacion[] compensacionesField;

    private decimal valorModificacionField;

    private string monedaField;

    private notaCreditoInfoNotaCreditoTotalImpuesto[] totalConImpuestosField;

    private string motivoField;

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
    public string rise
    {
        get
        {
            return this.riseField;
        }
        set
        {
            this.riseField = value;
        }
    }

    /// <remarks/>
    public byte codDocModificado
    {
        get
        {
            return this.codDocModificadoField;
        }
        set
        {
            this.codDocModificadoField = value;
        }
    }

    /// <remarks/>
    public string numDocModificado
    {
        get
        {
            return this.numDocModificadoField;
        }
        set
        {
            this.numDocModificadoField = value;
        }
    }

    /// <remarks/>
    public string fechaEmisionDocSustento
    {
        get
        {
            return this.fechaEmisionDocSustentoField;
        }
        set
        {
            this.fechaEmisionDocSustentoField = value;
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
    [System.Xml.Serialization.XmlArrayItemAttribute("compensacion", IsNullable = false)]
    public notaCreditoInfoNotaCreditoCompensacion[] compensaciones
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
    public decimal valorModificacion
    {
        get
        {
            return this.valorModificacionField;
        }
        set
        {
            this.valorModificacionField = value;
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
    [System.Xml.Serialization.XmlArrayItemAttribute("totalImpuesto", IsNullable = false)]
    public notaCreditoInfoNotaCreditoTotalImpuesto[] totalConImpuestos
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
    public string motivo
    {
        get
        {
            return this.motivoField;
        }
        set
        {
            this.motivoField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class notaCreditoInfoNotaCreditoCompensacion
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
public partial class notaCreditoInfoNotaCreditoTotalImpuesto
{

    private byte codigoField;

    private byte codigoPorcentajeField;

    private decimal baseImponibleField;

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
public partial class notaCreditoDetalle
{

    private string codigoInternoField;

    private string codigoAdicionalField;

    private string descripcionField;

    private decimal cantidadField;

    private decimal precioUnitarioField;

    private decimal descuentoField;

    private decimal precioTotalSinImpuestoField;

    private notaCreditoDetalleDetAdicional[] detallesAdicionalesField;

    private notaCreditoDetalleImpuesto[] impuestosField;

    /// <remarks/>
    public string codigoInterno
    {
        get
        {
            return this.codigoInternoField;
        }
        set
        {
            this.codigoInternoField = value;
        }
    }

    /// <remarks/>
    public string codigoAdicional
    {
        get
        {
            return this.codigoAdicionalField;
        }
        set
        {
            this.codigoAdicionalField = value;
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
    public notaCreditoDetalleDetAdicional[] detallesAdicionales
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
    public notaCreditoDetalleImpuesto[] impuestos
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
public partial class notaCreditoDetalleDetAdicional
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
public partial class notaCreditoDetalleImpuesto
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
public partial class notaCreditoMaquinaFiscal
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
public partial class notaCreditoCampoAdicional
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

