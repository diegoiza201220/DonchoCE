using EFModel.Interfaces;
using EFModel.Context;

namespace EFModel.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly DonchoContext _context;

    public IFacClienteRepository FacClienteR { get; }
    public IFacProductoRepository FacProductoR { get; }
    public IFacOrdenRepository FacOrdenR { get; }
    public IFacDetalleOrdenRepository FacDetalleOrdenR { get; }
    public ICelCertificadoRepository CelCertificadoR { get; }
    public ICelLogDocumentoRepository CelLogDocumentoR { get; }
    public ICelSecuenciaSriRepository CelSecuenciasSriR { get; }
    public IGenParametroRepository GenParametroR { get; }
    public IFacSecuenciaDiaRepository FacSecuenciaDiaR { get; }
    public IGenUsuarioRepository GenUsuarioR { get; }
    public ICelInfoTributariaRepository CelInfoTributariaR { get; }

    public UnitOfWork(DonchoContext context)
    {
        _context = context;
        FacClienteR = new FacClienteRepository(context);
        FacProductoR = new FacProductoRepository(context);
        FacOrdenR = new FacOrdenRepository(context);
        FacDetalleOrdenR = new FacDetalleOrdenRepository(context);
        CelCertificadoR = new CelCertificadoRepository(context);
        CelLogDocumentoR = new CelLogDocumentoRepository(context);
        CelSecuenciasSriR = new CelSecuenciaSriRepository(context);
        GenParametroR = new GenParametroRepository(context);
        FacSecuenciaDiaR = new FacSecuenciaDiaRepository(context);
        GenUsuarioR = new GenUsuarioRepository(context);
        CelInfoTributariaR = new CelInfoTributariaRepository(context);
    }

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public void Dispose()
        => _context.Dispose();
}
