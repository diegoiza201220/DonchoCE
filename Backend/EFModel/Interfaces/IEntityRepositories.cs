using EFModel.Models;

namespace EFModel.Interfaces;

public interface IFacClienteRepository : IRepository<FacCliente>
{
    Task<FacCliente?> GetByIdAsync(int id);
    Task<FacCliente?> GetByCedulaRucAsync(string cedulaRuc);
}

public interface IFacProductoRepository : IRepository<FacProducto>
{
    Task<IEnumerable<FacProducto>> GetActivosAsync();
    Task<IEnumerable<FacProducto>> GetByGrupoAsync(string grupo);
}

public interface IFacOrdenRepository : IRepository<FacOrden>
{
    Task<FacOrden?> GetWithDetallesAsync(int id);
    Task<IEnumerable<FacOrden>> GetByClienteAsync(int clienteId);
    Task<IEnumerable<FacOrden>> GetByFechas(int fechaini, int fechafin);
}

public interface IFacDetalleOrdenRepository : IRepository<FacDetalleOrden>
{
    Task<IEnumerable<FacDetalleOrden>> GetByOrdenAsync(int ordenId);
    Task<IEnumerable<FacDetalleOrden>> GetByFechasProductosVendidos(int fechaini, int fechafin);
}

//public interface IProductoRepository2 : IRepository<FacProducto> { }

public interface ICelCertificadoRepository : IRepository<CelCertificado> { }

public interface ICelLogDocumentoRepository : IRepository<CelLogDocumento> { }

public interface ICelSecuenciaSriRepository : IRepository<CelSecuenciaSri> { }

public interface IGenParametroRepository
{
    Task<IEnumerable<GenParametro>> GetAllAsync();
    Task<GenParametro?> GetByIdAsync(string id);
    Task AddAsync(GenParametro entity);
    void Update(GenParametro entity);
    void Delete(GenParametro entity);
}

public interface IFacSecuenciaDiaRepository : IRepository<FacSecuenciaDia>
{
    Task<FacSecuenciaDia?> GetByCodigoAsync(int codigo);
    FacSecuenciaDia? GetSecuencia();
    Task<FacSecuenciaDia?> GetSecuenciaAsync();
}

public interface IGenUsuarioRepository : IRepository<GenUsuario>
{
    Task<GenUsuario?> GetByIdAsync(int id);
    Task<GenUsuario?> ValidateLogin(string nombre, string password);
}