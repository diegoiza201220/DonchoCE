using EFModel.Context;
using EFModel.Interfaces;
using EFModel.Models;

namespace EFModel.Repositories;

public class UnitOfWork(DonchoContext context) : IUnitOfWork
{
    private readonly DonchoContext _context = context;

    public IFacClienteRepository FacClienteR { get; } = new FacClienteRepository(context);
    public IFacProductoRepository FacProductoR { get; } = new FacProductoRepository(context);
    public IFacOrdenRepository FacOrdenR { get; } = new FacOrdenRepository(context);
    public IFacDetalleOrdenRepository FacDetalleOrdenR { get; } = new FacDetalleOrdenRepository(context);
    public ICelCertificadoRepository CelCertificadoR { get; } = new CelCertificadoRepository(context);
    public ICelLogDocumentoRepository CelLogDocumentoR { get; } = new CelLogDocumentoRepository(context);
    public ICelSecuenciaSriRepository CelSecuenciasSriR { get; } = new CelSecuenciaSriRepository(context);
    public IGenParametroRepository GenParametroR { get; } = new GenParametroRepository(context);
    public IFacSecuenciaDiaRepository FacSecuenciaDiaR { get; } = new FacSecuenciaDiaRepository(context);
    public IGenUsuarioRepository GenUsuarioR { get; } = new GenUsuarioRepository(context);
    public ICelInfoTributariaRepository CelInfoTributariaR { get; } = new CelInfoTributariaRepository(context);
    public IGenCatalogoRepository GenCatalogoR { get; } = new GenCatalogoRepository(context);
    public IGenCatalogoDetalleRepository GenCatalogoDetalleR { get; } = new GenCatalogoDetalleRepository(context);
    public IGenFeriadoRepository GenFeriadoR { get; } = new GenFeriadoRepository(context);
    public IGenSucursalRepository GenSucursalR { get; } = new GenSucursalRepository (context);
    public IGenUsuarioSucursalRepository GenUsuarioSucursalR { get; } = new GenUsuarioSucursalRepository(context);

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public void Dispose()
        => _context.Dispose();
}
