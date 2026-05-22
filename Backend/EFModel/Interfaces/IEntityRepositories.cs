using EFModel.DTO;
using EFModel.DTO.Reportes;
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
    new Task<IEnumerable<FacProductoDTO>> GetAllAsync();
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
    Task<IEnumerable<RptProductosVendidosPorFechasDTO>> GetByFechasProductosVendidos(int fechaini, int fechafin);
}

//public interface IProductoRepository2 : IRepository<FacProducto> { }

public interface ICelCertificadoRepository : IRepository<CelCertificado> { }

public interface ICelLogDocumentoRepository : IRepository<CelLogDocumento> { }

public interface ICelSecuenciaSriRepository : IRepository<CelSecuenciaSri>
{
    CelSecuenciaSri GetByTipoDocumento(string id);
}

public interface IGenParametroRepository
{
    Task<IEnumerable<GenParametro>> GetAllAsync();
    Task<GenParametro?> GetByIdAsync(string id);
    Task AddAsync(GenParametro entity);
    void Update(GenParametro entity);
    void Delete(GenParametro entity);
    GenParametro GetById(string id);
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

public interface ICelInfoTributariaRepository : IRepository<CelInfoTributaria>
{
    CelInfoTributaria GetById(int id);
}

public interface IGenCatalogoRepository : IRepository<GenCatalogo>
{
    GenCatalogo GetById(int id);
    GenCatalogo GetByNombre(string codigo);
}

public interface IGenCatalogoDetalleRepository : IRepository<GenCatalogoDetalle>
{
    GenCatalogoDetalle GetById(int id);

    GenCatalogoDetalle GetByCodigo(string codigo);

    IEnumerable<GenCatalogoDetalle> GetByCatalogoNombre(string catalogonombre);
}