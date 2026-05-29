using ComprobantesElectronicos.DTO.Sri;
using System.Text;
using ComprobantesElectronicos.Utils;
using EFModel.DTO;

namespace ComprobantesElectronicos.Services;

public class ComprobanteService
{
    private readonly SriService _sriService;
    private readonly InfowareFirmaService _infowareFirmaService;
    public ComprobanteService(SriService sriService, InfowareFirmaService infowareFirmaService)
    {
        _sriService = sriService;
        _infowareFirmaService = infowareFirmaService;
    }
    public async Task<ResultadoEmisionDTO> EmitirFacturaAsync(FacOrdenDTO ordenDTO)
    {
        return await Task.Run(async () =>
        {
            try
            {

                // 1. Generar el XML del comprobante
                var entidadSri = ConvertirAEntidadSri.ObtenerFactura(ordenDTO);

                // 2. Firmar el XML con XAdES-BES
                var xmlFirmado = _infowareFirmaService.FirmarDocumento(entidadSri);

                // 3. Convertir a Base64 para enviar al SRI
                var xmlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlFirmado));

                // 4. Enviar al SRI
                var respuestaRecepcion = await _sriService.EnviarComprobanteAsync(xmlBase64);

                if (!respuestaRecepcion.FueRecibida)
                {
                    return new ResultadoEmisionDTO
                    {
                        Exitoso = false,
                        Mensajes = respuestaRecepcion.Mensajes.Select(m => m.Mensaje).ToList()
                    };
                }

                // 5. Esperar y consultar autorización (el SRI puede tardar unos segundos)
                await Task.Delay(30000);
                var claveAcceso = ordenDTO.ClaveNumeroAutorizacion; // método que arma la clave de 49 dígitos
                var respuestaAutorizacion = await _sriService.ConsultarAutorizacionAsync(claveAcceso);

                if (!respuestaAutorizacion.FueAutorizado)
                {
                    return new ResultadoEmisionDTO
                    {
                        Exitoso = false,
                        Mensajes = respuestaAutorizacion.Mensajes.Select(m => m.Mensaje).ToList()
                    };
                }

        // 7. Guardar el número de autorización en la orden
        //facOrden.NumeroAutorizacion = respuestaAutorizacion.NumeroAutorizacion;
        //facOrden.FechaAutorizacion = respuestaAutorizacion.FechaAutorizacion;
        //facOrden.XmlAutorizado = respuestaAutorizacion.XmlAutorizado;
        //_uow.FacOrdenR.Update(orden);
        //await _uow.SaveChangesAsync();

                return new ResultadoEmisionDTO
                {
                    Exitoso = true,
                    NumeroAutorizacion = respuestaAutorizacion.NumeroAutorizacion,
                    FechaAutorizacion = respuestaAutorizacion.FechaAutorizacion,
                    XmlAutorizado = respuestaAutorizacion.XmlAutorizado
                };
            }
            catch (Exception ex)
            {
                return new ResultadoEmisionDTO
                {
                    Exitoso = false,
                    Mensajes = new List<string> { $"Error al emitir comprobante: {ex.Message}" }
                };
            }
        });
    }

}
