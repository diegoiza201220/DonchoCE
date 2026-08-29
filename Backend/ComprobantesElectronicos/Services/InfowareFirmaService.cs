using EFModel.Interfaces;
using EFModel.Models;
using Infoware.SRI.Firmar;
using Microsoft.Extensions.Configuration;
using Utils;

namespace ComprobantesElectronicos.Services
{
    public class InfowareFirmaService(ICertificadoService certificadoService, IConfiguration configuration, ICacheService cache)
    {

        /// <summary>
        /// Genera el XML firmado de un documento (entidad del SRI) utilizando el certificado digital configurado en la aplicación.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entidad"></param>
        /// <returns></returns>
        public string FirmarDocumento<T>(T entidad)
        {
            GenParametro genParametroPathCertificado;
            GenParametro genParametroPwdCertificado;
            _ = cache.TryGet(Constantes.PATH_CERTIFICADO, out genParametroPathCertificado);
            _ = cache.TryGet(Constantes.PWD_CERTIFICADO, out genParametroPwdCertificado);
            var rutaCertificado = genParametroPathCertificado.Valor;
            var clave = genParametroPwdCertificado.Valor;
            certificadoService.CargarDesdeP12(rutaCertificado, clave);
            return certificadoService.FirmarDocumento(entidad).OuterXml;
        }

}
}
