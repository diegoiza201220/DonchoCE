using EFModel.DTO;
using EnvioCorreos.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnvioCorreos.Interfaces
{
    public interface IEmailService
    {
        Task<ResultadoEmail> EnviarAsync(EmailMessage mensaje, FacOrdenDTO facOrdenDTO);
        Task<ResultadoEmail> EnviarFacturaAsync(
            string destinatario,
            string nombreCliente,
            string numeroFactura,
            byte[] ridePdf,
            string xmlFirmado, FacOrdenDTO facOrdenDTO);
    }

    public class ResultadoEmail
    {
        public bool Exitoso { get; set; }
        public string Error { get; set; } = "";

        public static ResultadoEmail Ok()
            => new() { Exitoso = true };

        public static ResultadoEmail Fallo(string error)
            => new() { Exitoso = false, Error = error };
    }
}
