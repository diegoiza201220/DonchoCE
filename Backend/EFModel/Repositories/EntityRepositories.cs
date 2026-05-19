using Microsoft.EntityFrameworkCore;
using EFModel.Context;
using EFModel.Models;
using EFModel.Interfaces;


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

    public async Task<IEnumerable<FacDetalleOrden>> GetByFechasProductosVendidos(int fechaini, int fechafin)
        => await _dbSet.AsNoTracking()
            .Include(p => p.Producto)
            .Where(o => o.Orden.FechaInteger >= fechaini && o.Orden.FechaInteger <= fechafin)
            .ToListAsync();
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
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(g=> g.Id == id);

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
        return BCrypt.Net.BCrypt.Verify(password, usuario.Password)? usuario: null;
    }
}

// ── CelInfoTributaria──────────────────────────────────────────────────────────────────
public class CelInfoTributariaRepository : Repository<CelInfoTributaria>, ICelInfoTributariaRepository
{
    public CelInfoTributariaRepository(DonchoContext context) : base(context) { }

    public new CelInfoTributaria GetById(int id)
        => _dbSet.AsNoTracking().FirstOrDefault(c => c.Id == id);

}