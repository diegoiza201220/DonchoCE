using Infoware.SRI.Core.Helpers;
using Infoware.SRI.Firmar;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

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

        public string FirmarDocumento<T>(T xmlSinFirmar)
        {
            // Alias semántico para usar desde OrdenService
            var rutaCertificado = _config["FirmaElectronica:RutaCertificado"];
            var clave = _config["FirmaElectronica:Clave"];
            _certificadoService.CargarDesdeP12(rutaCertificado, clave);
            var xmlFirmado = _certificadoService.FirmarDocumento(xmlSinFirmar);

            //_certificadoService.
            //var xadesService = new XadesService();
            //var signatureDocument = xadesService.Sign(xmlSinFirmar, parametros);

            return xmlFirmado.OuterXml;
        }

    }
}
