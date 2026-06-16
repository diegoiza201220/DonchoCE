using EFModel.Context;
using EFModel.DTO;
using EFModel.DTO.Reportes;
using EFModel.Interfaces;
using EFModel.Models;
using Microsoft.EntityFrameworkCore;


namespace EFModel.Repositories;

// ── Cliente ──────────────────────────────────────────────────────────────────
public class FacClienteRepository : Repository<FacCliente>, IFacClienteRepository
{
    public FacClienteRepository(DonchoContext context) : base(context) { }

    public new async Task<FacCliente?> GetByIdAsync(int id)
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public async Task<FacCliente?> GetByCedulaRucAsync(string cedulaRuc)
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.CedulaRuc == cedulaRuc);
}

// ── Producto ──────────────────────────────────────────────────────────────────
public class FacProductoRepository : Repository<FacProducto>, IFacProductoRepository
{
    public FacProductoRepository(DonchoContext context) : base(context) { }

    public async Task<IEnumerable<FacProducto>> GetActivosAsync()
        => await _dbSet.AsNoTracking().Where(p => p.Activo).OrderBy(p => p.OrdenAparicion).ToListAsync();

    public async Task<IEnumerable<FacProducto>> GetByGrupoAsync(string grupo)
        => await _dbSet.AsNoTracking().Where(p => p.Grupo == grupo && p.Activo).ToListAsync();

    public async Task<IEnumerable<FacProductoDTO>> GetAllDtoAsync()
    {
        var resultado = (from p in _context.FacProducto
                         join d in _context.GenCatalogoDetalle
                         on p.CodigoIva equals d.Id
                         select new FacProductoDTO() { Activo = p.Activo, CodigoIva = p.CodigoIva, Grupo = p.Grupo, Id = p.Id, IvaTarifa = d.Codigo, IvaValor = p.Valor * Convert.ToDecimal(d.Codigo.Replace("%",""))/100, Nombre=p.Nombre, OrdenAparicion = p.OrdenAparicion, PedidoACocina = p.PedidoACocina, Valor = p.Valor, ValorDoncho = p.ValorDoncho, ValorTotal = p.Valor + p.Valor * Convert.ToDecimal(d.Codigo.Replace("%", "")) / 100 }
                         ).ToList().OrderByDescending(o=> o.Nombre);
        return resultado;
    }

    public async new Task<IEnumerable<FacProducto>> GetAllAsync() => await _dbSet.ToListAsync();

    public IEnumerable<FacProducto> GetAll() => [.. _dbSet];

    public IEnumerable<FacProductoDTO> GetAllDto() {
        var resultado = (from p in _context.FacProducto
                         join d in _context.GenCatalogoDetalle
                         on p.CodigoIva equals d.Id
                         select new FacProductoDTO() { Activo = p.Activo, CodigoIva = p.CodigoIva, Grupo = p.Grupo, Id = p.Id, IvaTarifa = d.Codigo, IvaValor = p.Valor * Convert.ToDecimal(d.Codigo.Replace("%", "")) / 100, Nombre = p.Nombre, OrdenAparicion = p.OrdenAparicion, PedidoACocina = p.PedidoACocina, Valor = p.Valor, ValorDoncho = p.ValorDoncho, ValorTotal = p.Valor + p.Valor * Convert.ToDecimal(d.Codigo.Replace("%", "")) / 100 }
                             ).ToList().OrderByDescending(o => o.Nombre);
        return resultado;
    }
}

// ── Orden ─────────────────────────────────────────────────────────────────────
public class FacOrdenRepository : Repository<FacOrden>, IFacOrdenRepository
{
    public FacOrdenRepository(DonchoContext context) : base(context) { }

    public async Task<FacOrden?> GetWithDetallesAsync(int id)
        => await _dbSet.AsNoTracking()
            .Include(o => o.Cliente)
            .Include(o => o.FacDetalleOrdens)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<IEnumerable<FacOrden>> GetByClienteAsync(int clienteId)
        => await _dbSet.AsNoTracking()
            .Include(o => o.FacDetalleOrdens)
            .Where(o => o.Clienteid == clienteId)
            .ToListAsync();
    public async Task<IEnumerable<FacOrden>> GetByFechas(int fechaini, int fechafin)
    => await _dbSet.AsNoTracking()
        .Include(o => o.Cliente)
        .Where(o => o.FechaInteger >= fechaini && o.FechaInteger <= fechafin)
        .OrderByDescending(o => o.Fecha)
        .ToListAsync();

    public async Task<IEnumerable<RptFacturasPorFechasDTO>> GetFacturasPorFecha(int fechaini, int fechafin)
    {
        var resultado = (from o in _context.FacOrden
                         join c in _context.FacCliente
                         on o.Clienteid equals c.Id
                         join l in _context.CelLogDocumento
                         on o.ClaveNumeroAutorizacion equals l.Autorizacion
                         where o.FechaInteger >= fechaini && o.FechaInteger <= fechafin
                         select new RptFacturasPorFechasDTO() 
                         {
                             Cliente = $"{c.Nombre} {c.Apellido}",
                             Establecimiento = o.Establecimiento,
                             Estado = l.Estado==0?"No Enviada":l.Estado==1?"Enviada":l.Estado==2?"Recibida":"Autorizada",
                             Fecha = o.Fecha,
                             ImpuestoPorcentaje = o.ImpuestoPorcentaje,
                             ImpuestoValor = o.ImpuestoValor,
                             NumeroAutorizacion = o.ClaveNumeroAutorizacion,
                             NumeroFactura = o.NumeroFactura,
                             PuntoEmision = o.PuntoEmision,
                             Secuencial = o.Secuencial,
                             TipoPago = o.TipoPago,
                             TotalOrden = o.TotalOrden,
                             TotalSinImpuestos = o.TotalSinImpuestos,
                             Mensaje = l.Estado == 200?"OK":l.Mensaje
                         }
                     ).ToList().OrderBy(o => o.NumeroFactura);
        return resultado;
    }

    public async Task<IEnumerable<RptDocumentosPorFechasDTO>> GetDocumentosPorFecha(int fechaini, int fechafin) 
        => _dbSet.AsNoTracking().Where(x=> x.FechaInteger >= fechaini && x.FechaInteger<=fechafin).GroupBy(x => x.EsFactura)
            .Select(g => new RptDocumentosPorFechasDTO
            {
                Documento = g.Key ? "Factura" : "Orden",
                Cantidad = g.Count()
            });
}

// ── DetalleOrden ──────────────────────────────────────────────────────────────
public class FacDetalleOrdenRepository : Repository<FacDetalleOrden>, IFacDetalleOrdenRepository
{
    public FacDetalleOrdenRepository(DonchoContext context) : base(context) { }

    public async Task<IEnumerable<FacDetalleOrden>> GetByOrdenAsync(int ordenId)
        => await _dbSet.AsNoTracking()
            .Include(d => d.Producto)
            .Where(d => d.Ordenid == ordenId)
            .ToListAsync();

    public async Task<IEnumerable<RptProductosVendidosPorFechasDTO>> GetByFechasProductosVendidos(int fechaini, int fechafin)
    {
        var detalles = _dbSet.AsNoTracking()
            .Include(p => p.Producto)
            .Where(o => o.Orden.FechaInteger >= fechaini && o.Orden.FechaInteger <= fechafin)
            .Select(g => new { g.Producto.Nombre, g.Cantidad });

        return detalles.GroupBy(c => c.Nombre)
                .Select(g => new
                {
                    g.Key,
                    SUM = g.Sum(s => s.Cantidad)
                }).OrderByDescending(o => o.SUM).Select(d => new RptProductosVendidosPorFechasDTO
                {
                    Plato = d.Key,
                    Cantidad = d.SUM
                });

    }
}

// ── Celcertificado ────────────────────────────────────────────────────────────
public class CelCertificadoRepository : Repository<CelCertificado>, ICelCertificadoRepository
{
    public CelCertificadoRepository(DonchoContext context) : base(context) { }
}

// ── CellogDocumento ───────────────────────────────────────────────────────────
public class CelLogDocumentoRepository : Repository<CelLogDocumento>, ICelLogDocumentoRepository
{
    public CelLogDocumentoRepository(DonchoContext context) : base(context) { }
}

// ── CelsecuenciaSri ───────────────────────────────────────────────────────────
public class CelSecuenciaSriRepository : Repository<CelSecuenciaSri>, ICelSecuenciaSriRepository
{
    public CelSecuenciaSriRepository(DonchoContext context) : base(context) { }
    public CelSecuenciaSri GetByTipoDocumento(string id)
        => _dbSet.AsNoTracking().FirstOrDefault(s => s.TipoDocumento == id);
}

// ── Genparametro (PK es string, repositorio propio) ───────────────────────────
public class GenParametroRepository : Repository<GenParametro>, IGenParametroRepository
{
    public GenParametroRepository(DonchoContext context) : base(context) { }
    public async Task<IEnumerable<GenParametro>> GetAllAsync()
        => await _dbSet.AsNoTracking().ToListAsync();

    public async Task<GenParametro?> GetByIdAsync(string id)
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id);

    public GenParametro GetById(string id)
    {
        return _dbSet.AsNoTracking().FirstOrDefault(g => g.Id == id);
    }
}

// ── Secuencium ────────────────────────────────────────────────────────────────
public class FacSecuenciaDiaRepository(DonchoContext context) : Repository<FacSecuenciaDia>(context), IFacSecuenciaDiaRepository
{
    public async Task<FacSecuenciaDia?> GetByCodigoAsync(int fecha)
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(s => s.Fecha == fecha);

    public FacSecuenciaDia? GetSecuencia()
    {
        return _dbSet.AsNoTracking().FirstOrDefault();
    }

    public async Task<FacSecuenciaDia?> GetSecuenciaAsync()
    => await _dbSet.AsNoTracking().FirstOrDefaultAsync();
}

// ── Usuario──────────────────────────────────────────────────────────────────
public class GenUsuarioRepository : Repository<GenUsuario>, IGenUsuarioRepository
{
    public GenUsuarioRepository(DonchoContext context) : base(context) { }

    public new async Task<GenUsuario?> GetByIdAsync(int id)
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public async Task<GenUsuario?> ValidateLogin(string nombre, string password)
    {
        var usuario = await _dbSet.FirstOrDefaultAsync(u => u.Nombre == nombre);
        if (usuario == null) return null;
        return BCrypt.Net.BCrypt.Verify(password, usuario.Password) ? usuario : null;
    }
}

// ── CelInfoTributaria──────────────────────────────────────────────────────────────────
public class CelInfoTributariaRepository : Repository<CelInfoTributaria>, ICelInfoTributariaRepository
{
    public CelInfoTributariaRepository(DonchoContext context) : base(context) { }

    public new CelInfoTributaria GetById(int id)
        => _dbSet.AsNoTracking().FirstOrDefault(c => c.Id == id);

}

// ── GenCatalogo ───────────────────────────────────────────────────────────
public class GenCatalogoRepository : Repository<GenCatalogo>, IGenCatalogoRepository
{
    public GenCatalogoRepository(DonchoContext context) : base(context) { }

    public new GenCatalogo GetById(int id)
    => _dbSet.AsNoTracking().FirstOrDefault(c => c.Id == id);

    public new GenCatalogo GetByNombre(string nombre)
=> _dbSet.AsNoTracking().FirstOrDefault(c => c.Nombre == nombre);
}

// ── GenCatalogo ───────────────────────────────────────────────────────────
public class GenCatalogoDetalleRepository : Repository<GenCatalogoDetalle>, IGenCatalogoDetalleRepository
{
    public GenCatalogoDetalleRepository(DonchoContext context) : base(context) { }

    public GenCatalogoDetalle GetById(int id)
    => _dbSet.AsNoTracking().FirstOrDefault(c => c.Id == id);

    public GenCatalogoDetalle GetByCodigo(string codigo)
        => _dbSet.AsNoTracking().FirstOrDefault(c => c.Codigo == codigo);
    public IEnumerable<GenCatalogoDetalle> GetByCatalogoNombre(string catalogonombre)
        => _dbSet.AsNoTracking()
        .Include(o => o.Catalogo)
        .Where(o => o.Catalogo.Nombre == catalogonombre)
        .ToList();
}

public class GenFeriadoRepository : Repository<GenFeriado>, IGenFeriadoRepository
{
    public GenFeriadoRepository(DonchoContext context) : base(context) { }

    public bool GetByFecha(int fechainteger)
    => _dbSet.Any(c => c.Fecha == fechainteger);
}