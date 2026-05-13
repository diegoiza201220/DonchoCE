namespace WebApiDonCho.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IFacClienteRepository FacClienteR { get; }
    IFacProductoRepository FacProductoR { get; }
    IFacOrdenRepository FacOrdenR { get; }
    IFacDetalleOrdenRepository FacDetalleOrdenR { get; }
    ICelCertificadoRepository CelCertificadoR { get; }
    ICelLogDocumentoRepository CelLogDocumentoR { get; }
    ICelSecuenciaSriRepository CelSecuenciasSriR { get; }
    IGenParametroRepository GenParametroR { get; }
    IFacSecuenciaDiaRepository FacSecuenciaDiaR { get; }
    IGenUsuarioRepository GenUsuarioR { get; }

    Task<int> SaveChangesAsync();
}
