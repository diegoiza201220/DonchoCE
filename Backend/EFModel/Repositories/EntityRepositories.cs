using EFModel.Context;
using EFModel.DTO;
using EFModel.DTO.Reportes;
using EFModel.Interfaces;
using EFModel.Models;
using Microsoft.EntityFrameworkCore;


namespace EFModel.Repositories;

// ── Cliente ──────────────────────────────────────────────────────────────────
public class FacClienteRepository(DonchoContext context) : Repository<FacCliente>(context), IFacClienteRepository
{
    public new async Task<FacCliente?> GetByIdAsync(int id)
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public async Task<FacCliente?> GetByCedulaRucAsync(string cedulaRuc)
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.CedulaRuc == cedulaRuc);
}

// ── Producto ──────────────────────────────────────────────────────────────────
public class FacProductoRepository(DonchoContext context) : Repository<FacProducto>(context), IFacProductoRepository
{
    public async Task<IEnumerable<FacProducto>> GetActivosAsync()
        => await _dbSet.AsNoTracking().Where(p => p.Activo).OrderBy(p => p.OrdenAparicion).ToListAsync();

    public async Task<IEnumerable<FacProducto>> GetByGrupoAsync(string grupo)
        => await _dbSet.AsNoTracking().Where(p => p.Grupo == grupo && p.Activo).ToListAsync();

    public async Task<IEnumerable<FacProductoDTO>> GetAllDtoAsync()
    {
        var resultado = (from p in _context.FacProducto
                         join d in _context.GenCatalogoDetalle
                         on p.CodigoIva equals d.Id
                         select new FacProductoDTO() { Activo = p.Activo, CodigoIva = p.CodigoIva, Grupo = p.Grupo, Id = p.Id, IvaTarifa = d.Codigo, IvaValor = p.Valor * Convert.ToDecimal(d.Codigo.Replace("%", "")) / 100, Nombre = p.Nombre, OrdenAparicion = p.OrdenAparicion, PedidoACocina = p.PedidoACocina, Valor = p.Valor, ValorDoncho = p.ValorDoncho, ValorTotal = p.Valor + p.Valor * Convert.ToDecimal(d.Codigo.Replace("%", "")) / 100 }
                         ).ToList().OrderByDescending(o => o.Nombre);
        return resultado;
    }

    public async new Task<IEnumerable<FacProducto>> GetAllAsync() => await _dbSet.ToListAsync();

    public IEnumerable<FacProducto> GetAll() => [.. _dbSet];

    public IEnumerable<FacProductoDTO> GetAllDto()
    {
        var resultado = (from p in _context.FacProducto
                         join d in _context.GenCatalogoDetalle
                         on p.CodigoIva equals d.Id
                         select new FacProductoDTO() { Activo = p.Activo, CodigoIva = p.CodigoIva, Grupo = p.Grupo, Id = p.Id, IvaTarifa = d.Codigo, IvaValor = p.Valor * Convert.ToDecimal(d.Codigo.Replace("%", "")) / 100, Nombre = p.Nombre, OrdenAparicion = p.OrdenAparicion, PedidoACocina = p.PedidoACocina, Valor = p.Valor, ValorDoncho = p.ValorDoncho, ValorTotal = p.Valor + p.Valor * Convert.ToDecimal(d.Codigo.Replace("%", "")) / 100 }
                             ).ToList().OrderByDescending(o => o.Nombre);
        return resultado;
    }
}

// ── Orden ─────────────────────────────────────────────────────────────────────
public class FacOrdenRepository(DonchoContext context) : Repository<FacOrden>(context), IFacOrdenRepository
{
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
                             Estado = l.Estado == 0 ? "No Enviada" : l.Estado == 1 ? "Enviada" : l.Estado == 2 ? "Recibida" : "Autorizada",
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
                             Mensaje = l.Estado == 200 ? "OK" : l.Mensaje
                         }
                     ).ToList().OrderBy(o => o.NumeroFactura);
        return resultado;
    }

    public async Task<IEnumerable<RptDocumentosPorFechasDTO>> GetDocumentosPorFecha(int fechaini, int fechafin)
        => _dbSet.AsNoTracking().Where(x => x.FechaInteger >= fechaini && x.FechaInteger <= fechafin).GroupBy(x => x.EsFactura)
            .Select(g => new RptDocumentosPorFechasDTO
            {
                Documento = g.Key ? "Factura" : "Orden",
                Cantidad = g.Count()
            });
}

// ── DetalleOrden ──────────────────────────────────────────────────────────────
public class FacDetalleOrdenRepository(DonchoContext context) : Repository<FacDetalleOrden>(context), IFacDetalleOrdenRepository
{
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
public class CelCertificadoRepository(DonchoContext context) : Repository<CelCertificado>(context), ICelCertificadoRepository
{
}

// ── CellogDocumento ───────────────────────────────────────────────────────────
public class CelLogDocumentoRepository(DonchoContext context) : Repository<CelLogDocumento>(context), ICelLogDocumentoRepository
{
}

// ── CelsecuenciaSri ───────────────────────────────────────────────────────────
public class CelSecuenciaSriRepository(DonchoContext context) : Repository<CelSecuenciaSri>(context), ICelSecuenciaSriRepository
{
    public CelSecuenciaSri GetByTipoDocumento(string id)
        => _dbSet.AsNoTracking().FirstOrDefault(s => s.TipoDocumento == id);
}

// ── Genparametro (PK es string, repositorio propio) ───────────────────────────
public class GenParametroRepository(DonchoContext context) : Repository<GenParametro>(context), IGenParametroRepository
{
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
public class GenUsuarioRepository(DonchoContext context) : Repository<GenUsuario>(context), IGenUsuarioRepository
{
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
public class CelInfoTributariaRepository(DonchoContext context) : Repository<CelInfoTributaria>(context), ICelInfoTributariaRepository
{
    public CelInfoTributaria GetById(int id)
        => _dbSet.AsNoTracking().FirstOrDefault(c => c.Id == id);

}

// ── GenCatalogo ───────────────────────────────────────────────────────────
public class GenCatalogoRepository(DonchoContext context) : Repository<GenCatalogo>(context), IGenCatalogoRepository
{
    public GenCatalogo GetById(int id)
    {
        return _dbSet.AsNoTracking().FirstOrDefault(c => c.Id == id);
    }

    public new GenCatalogo GetByNombre(string nombre)
=> _dbSet.AsNoTracking().FirstOrDefault(c => c.Nombre == nombre);
}

// ── GenCatalogo ───────────────────────────────────────────────────────────
public class GenCatalogoDetalleRepository(DonchoContext context) : Repository<GenCatalogoDetalle>(context), IGenCatalogoDetalleRepository
{
    public GenCatalogoDetalle GetById(int id)
    => _dbSet.AsNoTracking().FirstOrDefault(c => c.Id == id);

    public GenCatalogoDetalle GetByCodigo(string codigo)
        => _dbSet.AsNoTracking().FirstOrDefault(c => c.Codigo == codigo);
    public IEnumerable<GenCatalogoDetalle> GetByCatalogoNombre(string catalogonombre)
        => [.. _dbSet.AsNoTracking()
        .Include(o => o.Catalogo)
        .Where(o => o.Catalogo.Nombre == catalogonombre)];
}

public class GenFeriadoRepository(DonchoContext context) : Repository<GenFeriado>(context), IGenFeriadoRepository
{
    public bool GetByFecha(int fechainteger)
    => _dbSet.Any(c => c.Fecha == fechainteger);
}

public class GenSucursalRepository(DonchoContext context) : Repository<GenSucursal>(context), IGenSucursalRepository
{
    public GenSucursal GetById(int id)
    {
        return _dbSet.AsNoTracking().FirstOrDefault(g => g.Id == id);
    }

    public IEnumerable<GenSucursal> GetAll()
    => [.. _dbSet.AsNoTracking()];
}

public class GenUsuarioSucursalRepository(DonchoContext context) : Repository<GenUsuarioSucursal>(context), IGenUsuarioSucursalRepository
{
    public IEnumerable<GenUsuarioSucursal> GetByUsuarioId(int id)
    {
            //public async Task<IEnumerable<FacDetalleOrden>> GetByOrdenAsync(int ordenId)
       // => await
       //
       var l = _dbSet.AsNoTracking()
            .Include(d => d.Sucursal)
            .Where(d => d.Usuarioid == id)
            .ToList();
        return l;
//        return [.. _dbSet.AsNoTracking().Where(x => x.Usuarioid == id)];
    }

    public async Task<IEnumerable<GenUsuarioSucursal>> GetBySucursalId(int id)
    => await _dbSet.AsNoTracking().Where(x=> x.Sucursalid == id).ToListAsync();
}