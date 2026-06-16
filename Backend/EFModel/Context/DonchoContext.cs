using Microsoft.EntityFrameworkCore;
using EFModel.Models;

namespace EFModel.Context;

public partial class DonchoContext : DbContext
{
    public DonchoContext()
    {
    }

    public DonchoContext(DbContextOptions<DonchoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CelCertificado> CelCertificado { get; set; }

    public virtual DbSet<CelLogDocumento> CelLogDocumento { get; set; }

    public virtual DbSet<CelSecuenciaSri> CelSecuenciaSri { get; set; }

    public virtual DbSet<FacCliente> FacCliente { get; set; }

    public virtual DbSet<FacDetalleOrden> FacDetalleOrden { get; set; }

    public virtual DbSet<FacOrden> FacOrden { get; set; }

    public virtual DbSet<FacSecuenciaDia> FacSecuenciaDia { get; set; }

    public virtual DbSet<GenParametro> GenParametro { get; set; }

    public virtual DbSet<FacProducto> FacProducto { get; set; }

    public virtual DbSet<GenUsuario> GenUsuario { get; set; }

    public virtual DbSet<CelInfoTributaria> CelInfoTributaria { get; set; }

    public virtual DbSet<GenCatalogo> GenCatalogo { get; set; }

    public virtual DbSet<GenCatalogoDetalle> GenCatalogoDetalle { get; set; }
    public virtual DbSet<GenFeriado> GenFeriado { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=postgres;Username=postgres;Password=postgres1234");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CelCertificado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("celcertificado_pk");

            entity.ToTable("cel_certificado");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Clave)
                .HasColumnType("character varying")
                .HasColumnName("clave");
            entity.Property(e => e.Firma).HasColumnName("firma");
            entity.Property(e => e.NombreCertificado)
                .HasColumnType("character varying")
                .HasColumnName("nombre_certificado");
        });

        modelBuilder.Entity<CelLogDocumento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cellog_documentos_pk");

            entity.ToTable("cel_log_documento");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Ambiente).HasColumnName("ambiente");
            entity.Property(e => e.Autorizacion)
                .HasColumnType("character varying")
                .HasColumnName("autorizacion");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.Mensaje)
                .HasColumnType("character varying")
                .HasColumnName("mensaje");
            entity.Property(e => e.TipoDocumento).HasColumnName("tipo_documento");
            entity.Property(e => e.TipoEmision).HasColumnName("tipo_emision");
            entity.Property(e => e.XmlFirmado).HasColumnName("xml_firmado");
        });

        modelBuilder.Entity<CelSecuenciaSri>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("celsecuencia_sri_pk");

            entity.ToTable("cel_secuencia_sri");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Establecimiento)
                .HasColumnType("character varying")
                .HasColumnName("establecimiento");
            entity.Property(e => e.Estado).HasColumnName("estado");
            entity.Property(e => e.PuntoDeEmision)
                .HasColumnType("character varying")
                .HasColumnName("punto_de_emision");
            entity.Property(e => e.SecuenciaActual).HasColumnName("secuencia_actual");
            entity.Property(e => e.TipoDocumento).HasColumnName("tipo_documento");
        });

        modelBuilder.Entity<FacCliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cliente_pk");

            entity.ToTable("fac_cliente");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Apellido)
                .HasColumnType("character varying")
                .HasColumnName("apellido");
            entity.Property(e => e.CedulaRuc)
                .HasColumnType("character varying")
                .HasColumnName("cedula_ruc");
            entity.Property(e => e.Direccion)
                .HasColumnType("character varying")
                .HasColumnName("direccion");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.FechaCumpleanios).HasColumnName("fecha_cumpleanios");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("now()")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.Nombre)
                .HasColumnType("character varying")
                .HasColumnName("nombre");
            entity.Property(e => e.TelefonoCelular)
                .HasColumnType("character varying")
                .HasColumnName("telefono_celular");
            entity.Property(e => e.UsuarioRegistro)
                .HasColumnType("character varying")
                .HasColumnName("usuario_registro");
        });

        modelBuilder.Entity<FacDetalleOrden>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("detalle_orden_pk");

            entity.ToTable("fac_detalle_orden");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Ordenid).HasColumnName("ordenid");
            entity.Property(e => e.PedidoACocina).HasColumnName("pedido_a_cocina");
            entity.Property(e => e.PrecioTotal).HasColumnName("precio_total");
            entity.Property(e => e.PrecioUnitario).HasColumnName("precio_unitario");
            entity.Property(e => e.Productoid).HasColumnName("productoid");
            entity.Property(e => e.ImpuestoValor).HasColumnName("impuesto_valor");
            entity.Property(e => e.ImpuestoTarifa).HasColumnName("impuesto_tarifa");
            entity.Property(e => e.ImpuestoCodigoPorcentaje).HasColumnName("impuesto_codigo_porcentaje");
            entity.Property(e => e.ImpuestoCodigo).HasColumnName("impuesto_codigo");

            entity.HasOne(d => d.Orden).WithMany(p => p.FacDetalleOrdens)
                .HasForeignKey(d => d.Ordenid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("detalle_orden_orden_fk");

            //entity.HasOne(d => d.Producto).WithMany(p => p.FacDetalleOrdens)
            //    .HasForeignKey(d => d.Productoid)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("detalle_orden_producto_fk");
        });

        modelBuilder.Entity<FacOrden>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orden_pk");

            entity.ToTable("fac_orden");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Clienteid).HasColumnName("clienteid");
            entity.Property(e => e.ImpuestoCodigo).HasColumnName("impuesto_codigo");
            entity.Property(e => e.ImpuestoCodigoPorcentaje).HasColumnName("impuesto_codigo_porcentaje");
            entity.Property(e => e.ImpuestoValor).HasColumnName("impuesto_valor");
            entity.Property(e => e.ClaveNumeroAutorizacion).HasColumnName("clave_numero_autorizacion");
            entity.Property(e => e.Establecimiento).HasColumnName("establecimiento");
            entity.Property(e => e.PuntoEmision).HasColumnName("punto_emision");
            entity.Property(e => e.EsFactura).HasColumnName("es_factura");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("now()")
                .HasColumnName("fecha");
            entity.Property(e => e.FechaInteger).HasColumnName("fecha_integer");
            entity.Property(e => e.NumeroFactura)
                .HasColumnType("character varying")
                .HasColumnName("numero_factura");
            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.TipoPago)
                .HasColumnType("character varying")
                .HasColumnName("tipo_pago");
            entity.Property(e => e.TotalOrden).HasColumnName("total_orden");
            entity.Property(e => e.TotalSinImpuestos).HasColumnName("total_sin_impuestos");
            entity.Property(e => e.ImpuestoPorcentaje).HasColumnName("impuesto_porcentaje");
            entity.Property(e => e.UsuarioRegistro)
                .HasColumnType("character varying")
                .HasColumnName("usuario_registro");
            entity.Property(e => e.ImpuestoBaseImponible).HasColumnName("impuesto_base_imponible");
            entity.Property(e => e.DocumentoPago)
                .HasColumnType("character varying")
                .HasColumnName("documento_pago");
            entity.Property(e => e.EsNotaCredito)
                .HasColumnName("es_nota_credito");
            entity.Property(e => e.NotaCreditoNumeroNotaCredito)
                .HasColumnType("character varying")
                .HasColumnName("nota_credito_numero_nota_credito");
            entity.Property(e => e.NotaCreditoClaveNumeroAutorizacion)
                .HasColumnType("character varying")
                .HasColumnName("nota_credito_clave_numero_autorizacion");
            entity.Property(e => e.NotaCreditoMotivo)
                .HasColumnType("character varying")
                .HasColumnName("nota_credito_motivo");
            entity.Property(e => e.NotaCreditoFecha)
                .HasColumnName("nota_credito_fecha");
            entity.HasOne(d => d.Cliente).WithMany(p => p.FacOrdens)
                .HasForeignKey(d => d.Clienteid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orden_cliente_fk");
        });

        modelBuilder.Entity<FacSecuenciaDia>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("secuencia_pk");

            entity.ToTable("fac_secuencia_dia");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.Secuencia).HasColumnName("secuencia");
        });

        modelBuilder.Entity<GenParametro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("genparametros_pk");

            entity.ToTable("gen_parametro");

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.Descripcion)
                .HasColumnType("character varying")
                .HasColumnName("descripcion");
            entity.Property(e => e.Valor)
                .HasColumnType("character varying")
                .HasColumnName("valor");
        });

        modelBuilder.Entity<FacProducto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("producto_pk");

            entity.ToTable("fac_producto");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.CodigoIva).HasColumnName("codigo_iva");
            entity.Property(e => e.Grupo)
                .HasColumnType("character varying")
                .HasColumnName("grupo");
            entity.Property(e => e.Nombre)
                .HasColumnType("character varying")
                .HasColumnName("nombre");
            entity.Property(e => e.OrdenAparicion)
                .HasDefaultValue((short)0)
                .HasColumnName("orden_aparicion");
            entity.Property(e => e.PedidoACocina)
                .HasDefaultValue(false)
                .HasColumnName("pedido_a_cocina");
            entity.Property(e => e.Valor).HasColumnName("valor");
            entity.Property(e => e.ValorDoncho).HasColumnName("valor_doncho");
        });

        modelBuilder.Entity<GenUsuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usuario_pk");

            entity.ToTable("gen_usuario");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasColumnType("character varying")
                .HasColumnName("nombre");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
        });

        modelBuilder.Entity<CelInfoTributaria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cel_info_tributaria_pk");

            entity.ToTable("cel_info_tributaria");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.RazonSocial)
                .HasColumnType("character varying")
                .HasColumnName("razon_social");
            entity.Property(e => e.NombreComercial)
                .HasColumnType("character varying")
                .HasColumnName("nombre_comercial");
            entity.Property(e => e.Ruc)
                .HasColumnType("character varying")
                .HasColumnName("ruc");
            entity.Property(e => e.DireccionMatriz)
                .HasColumnType("character varying")
                .HasColumnName("direccion_matriz");
            entity.Property(e => e.ContribuyenteEspecial)
                .HasColumnType("character varying")
                .HasColumnName("contribuyente_especial");
            entity.Property(e => e.ObligadoContabilidad)
                .HasColumnName("obligado_contabilidad");
            entity.Property(e => e.ContribuyenteRimpe)
                .HasColumnName("contribuyente_rimpe");
        });

        modelBuilder.Entity<GenCatalogo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("gen_catalogo_pk");

            entity.ToTable("gen_catalogo");

            entity.Property(e => e.Id)
                .HasColumnName("id");
            entity.Property(e => e.Nombre)
                .HasColumnType("character varying")
                .HasColumnName("nombre");
            entity.Property(e => e.Activo)
                .HasColumnName("activo");
        });

        modelBuilder.Entity<GenCatalogoDetalle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("gen_catalogo_detalle_pk");

            entity.ToTable("gen_catalogo_detalle");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Catalogoid).HasColumnName("catalogoid");
            entity.Property(e => e.Codigo).HasColumnName("codigo");
            entity.Property(e => e.Valor).HasColumnName("valor");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.HasOne(d => d.Catalogo).WithMany(p => p.CatalogoDetalles)
                .HasForeignKey(d => d.Catalogoid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("gen_catalogo_detalle_gen_catalogo_fk");
        });

        modelBuilder.Entity<GenFeriado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("gen_feriado_pk");

            entity.ToTable("gen_feriado");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
