using EFModel.Interfaces;
using EFModel.Models;
using EFModel.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiDonCho.Helpers.ComprobantesElectronicos
{
    public class CalcularInfoTributaria
    {
        public static (string ambiente, string tipoEmision, string secuencial, string claveAcceso) CalcularAmbienteYTipoEmision(bool esProduccion, DateTime fecha, string codDoc, CelInfoTributaria celInfoTributaria, CelSecuenciaSri celSecuenciaSri)
        {
            // Para este ejemplo, asumimos que el ambiente es 1 (producción) o 2 (pruebas)
            // y el tipo de emisión es 1 (normal) para ambos casos.
            string ambiente = !esProduccion ? "1" : "2";
            string tipoEmision = "1"; // Normal


            string claveAcceso = ClaveAcceso(ambiente, fecha, celInfoTributaria.Ruc, codDoc, celSecuenciaSri.SecuenciaActual.ToString("D9"), tipoEmision, codDoc, ambiente, $"{celSecuenciaSri.Establecimiento}{celSecuenciaSri.PuntoDeEmision}");
            return (ambiente, tipoEmision, celSecuenciaSri.SecuenciaActual.ToString("D9"), claveAcceso);
        }

        private static string ClaveAcceso(string ambiente, DateTime fecha, string ruc, string codDoc, string secuencial, string tipoEmision, string tipocomprobante, string tipoAmbiente, string establecimiento_ptoemi)
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
            string clave_acceso_sin_digito_verificador = $"{fecha.ToString("ddMMyyyy")}{tipocomprobante}{ruc}{tipoAmbiente}{establecimiento_ptoemi}{secuencial}{codigoNumerico}{tipoEmision}";
            int digitoVerificador = CalcularDigitoVerificador(clave_acceso_sin_digito_verificador);
            return $"{clave_acceso_sin_digito_verificador}{digitoVerificador}";
        }

        private static int CalcularDigitoVerificador(string claveAccesoSinDigito)
        {
            var clave1 = claveAccesoSinDigito.ToCharArray();
            int suma = 0, factor = 7;

            foreach (var item in clave1)
            {

                suma = suma + Convert.ToInt32(item.ToString()) * factor;
                factor = factor - 1;
                if (factor == 1)
                    factor = 7;

            }
            var digitoverificador = (suma % 11);
            digitoverificador = 11 - digitoverificador;
            if (digitoverificador == 11)
                digitoverificador = 0;
            else if (digitoverificador == 10)
                digitoverificador = 1;

            return digitoverificador ;
        }
    }
}
