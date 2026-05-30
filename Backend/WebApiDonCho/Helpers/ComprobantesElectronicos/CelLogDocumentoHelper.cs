using EFModel.DTO;
using EFModel.Models;
using Infoware.SRI.Core.Enumerados;

namespace WebApiDonCho.Helpers.ComprobantesElectronicos
{
    public static class CelLogDocumentoHelper
    {
        public static CelLogDocumento CrearLogInicial(FacOrdenDTO ordenDTO)
        {
            CelLogDocumento documento = new()
            {
                TipoDocumento = int.Parse(ordenDTO.CodDoc),
                Estado = 0,
                Mensaje = string.Empty,
                Autorizacion = ordenDTO.ClaveNumeroAutorizacion,
                XmlFirmado = string.Empty,
                Ambiente = 0,
                TipoEmision = 0
            };
            return documento;
        }
    }
}
