using EFModel.DTO;
using EFModel.Models;

namespace WebApiDonCho.Helpers.ComprobantesElectronicos
{
    public class InfoTributariaHelper
    {
        public static void SetInformacion(FacOrdenDTO ordenDTO, FacOrden orden, CelInfoTributaria celInfoTributaria, CelSecuenciaSri celSecuenciaSri, bool esProduccion, int coddoc)
        {
            // Para este ejemplo, asumimos que el ambiente es 1 (producción) o 2 (pruebas)
            // y el tipo de emisión es 1 (normal) para ambos casos.            
            string ambiente = !esProduccion ? "1" : "2";
            string tipoEmision = "1"; // Normal
            switch (coddoc)
            {
                case 1: //factura
                    DateTime localDate = ordenDTO.Fecha.ToLocalTime();
                    string claveAcceso = GetClaveAcceso(localDate, celInfoTributaria.Ruc, celSecuenciaSri.SecuenciaActual.ToString("D9"), tipoEmision, ordenDTO.CodDoc, ambiente, $"{celSecuenciaSri.Establecimiento}{celSecuenciaSri.PuntoDeEmision}");
                    orden.ClaveNumeroAutorizacion = ordenDTO.ClaveNumeroAutorizacion = claveAcceso;
                    orden.NumeroFactura = ordenDTO.NumeroFactura = celSecuenciaSri.SecuenciaActual.ToString("D9");
                    orden.Establecimiento = ordenDTO.Establecimiento = celSecuenciaSri.Establecimiento;
                    orden.PuntoEmision = ordenDTO.PuntoEmision = celSecuenciaSri.PuntoDeEmision;
                    break;

                case 2: //notacredito
                    DateTime nclocalDate = ordenDTO.NotaCreditoFecha.ToLocalTime();
                    string ncclaveAcceso = GetClaveAcceso(nclocalDate, celInfoTributaria.Ruc, celSecuenciaSri.SecuenciaActual.ToString("D9"), tipoEmision, "03", ambiente, $"{celSecuenciaSri.Establecimiento}{celSecuenciaSri.PuntoDeEmision}");
                    orden.NotaCreditoClaveNumeroAutorizacion = ordenDTO.NotaCreditoClaveNumeroAutorizacion = ncclaveAcceso;
                    orden.NotaCreditoNumeroNotaCredito = ordenDTO.NotaCreditoNumeroNotaCredito = celSecuenciaSri.SecuenciaActual.ToString("D9");
                    break;
                default:
                    break;
            }
            ordenDTO.RazonSocial = celInfoTributaria.RazonSocial;
            ordenDTO.NombreComercial = celInfoTributaria.NombreComercial;
            ordenDTO.RucDonCho = celInfoTributaria.Ruc;
            ordenDTO.Direccionmatriz = celInfoTributaria.DireccionMatriz;
            ordenDTO.ContribuyenteRimpe = celInfoTributaria.ContribuyenteRimpe;
            ordenDTO.ContribuyenteEspecial = celInfoTributaria.ContribuyenteEspecial;
            ordenDTO.DireccionEstablecimiento = celInfoTributaria.DireccionMatriz;
            ordenDTO.ObligadoContabilidad = celInfoTributaria.ObligadoContabilidad ? "SI" : "NO";
        }

        private static string GetClaveAcceso(DateTime fecha, string ruc, string secuencial, string tipoEmision, string tipocomprobante, string tipoAmbiente, string establecimiento_ptoemi)
        {
            //15052026 01 1714802681001 1 001001 000000003 12345678 1 3
            //15052026 - fecha de emisión ddmmaaaa
            //01 - tipo de comrobante tabla 3
            //1714802681001 - ruc
            //1 - tipo de ambiente
            //001001 - serie ptoemi y estab
            //000000003 - número del documento
            //1 - tipo de emisión
            //12345678 - código numérico aleatorio
            //1 - tipo de emision
            //3 - digito verificador (calculado con módulo 11 sobre los 48 dígitos anteriores)

            string codigoNumerico = new Random().Next(10000000, 99999999).ToString(); // Generar un código numérico aleatorio de 8 dígitos
            string clave_acceso_sin_digito_verificador = $"{fecha:ddMMyyyy}{tipocomprobante}{ruc}{tipoAmbiente}{establecimiento_ptoemi}{secuencial}{codigoNumerico}{tipoEmision}";
            return $"{clave_acceso_sin_digito_verificador}{CalcularDigitoVerificador(clave_acceso_sin_digito_verificador)}";
        }

        private static int CalcularDigitoVerificador(string claveAccesoSinDigito)
        {
            var clave1 = claveAccesoSinDigito.ToCharArray();
            int suma = 0, factor = 7;

            foreach (var item in clave1)
            {
                suma += Convert.ToInt32(item.ToString()) * factor;
                factor--;
                if (factor == 1)
                    factor = 7;
            }

            var digitoverificador = (suma % 11);
            digitoverificador = 11 - digitoverificador;
            if (digitoverificador == 11)
                digitoverificador = 0;
            else if (digitoverificador == 10)
                digitoverificador = 1;

            return digitoverificador;
        }
    }
}
