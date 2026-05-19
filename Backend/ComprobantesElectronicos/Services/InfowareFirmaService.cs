using Infoware.SRI.Firmar;
using Microsoft.Extensions.Configuration;

namespace ComprobantesElectronicos.Services
{
    public class InfowareFirmaService
    {
        private readonly ICertificadoService _certificadoService;
        private readonly IConfiguration _config;

        public InfowareFirmaService(ICertificadoService certificadoService, IConfiguration configuration )
        {
            _certificadoService = certificadoService;
            _config = configuration;
        }

        /// <summary>
        /// Genera el XML firmado de un documento (entidad del SRI) utilizando el certificado digital configurado en la aplicación.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entidad"></param>
        /// <returns></returns>
        public string FirmarDocumento<T>(T entidad)
        {
            var rutaCertificado = _config["FirmaElectronica:RutaCertificado"];
            var clave = _config["FirmaElectronica:Clave"];
            _certificadoService.CargarDesdeP12(rutaCertificado, clave);
            return _certificadoService.FirmarDocumento(entidad).OuterXml;
        }

    }
}
