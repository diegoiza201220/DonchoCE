using EFModel.DTO;
using EFModel.Models;

namespace WebApiDonCho.Helpers.ComprobantesElectronicos
{
    public static class CelLogDocumentoHelper
    {
        public static CelLogDocumento CrearLogInicial(FacOrdenDTO ordenDTO)
        {
            CelLogDocumento documento = new()
            {
                TipoDocumento = ordenDTO.EsNotaCredito ? 3 : int.Parse(ordenDTO.CodDoc),
                Estado = 0,
                Mensaje = string.Empty,
                Autorizacion = ordenDTO.EsNotaCredito ? ordenDTO.NotaCreditoClaveNumeroAutorizacion : ordenDTO.ClaveNumeroAutorizacion,
                XmlFirmado = string.Empty,
                Ambiente = 0,
                TipoEmision = 0,
                SucursalId = ordenDTO.Sucursalid,
                DocumentoId = ordenDTO.Id
            };
            return documento;
        }
    }
}
