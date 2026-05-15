using ComprobantesElectronicos.DTO.Sri;
using EFModel.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace ComprobantesElectronicos.Services;

public class ComprobanteService
{
    private readonly IConfiguration _config;
    public ComprobanteService(IConfiguration config)
    {
        _config = config;
        //_esProd = bool.Parse(config["Sri:Produccion"] ?? "false");
    }
    public async Task<ResultadoEmisionDTO> EmitirFacturaAsync(FacOrden facOrden)
    {
        // 1. Obtener la orden
        //var orden = await _uow.FacOrdenR.GetWithDetallesAsync(ordenId);
        if (facOrden is null) throw new InvalidOperationException("Orden no encontrada.");

        // 2. Generar el XML del comprobante
        FirmaElectronicaService SriService = new FirmaElectronicaService(_config);
        var xmlFirmado = SriService.GenerarXMLFacturaFirmado(facOrden);

        // 3. Firmar el XML con XAdES-BES
        //var xmlFirmado = _firma.FirmarXml(xmlPlano);

        // 4. Convertir a Base64 para enviar al SRI
        byte[] xmlFirmadoBytesOriginal = Encoding.UTF8.GetBytes(xmlFirmado);

        string xmlBase64 = Convert.ToBase64String(xmlFirmadoBytesOriginal);

        byte[] xmlFirmadoBytesBase64 = Encoding.UTF8.GetBytes(xmlBase64);
        // 5. Enviar al SRI
        SriService sriService = new SriService(_config);
        var respuestaRecepcion = await sriService.EnviarComprobanteAsync(xmlFirmadoBytesBase64);
        if (!respuestaRecepcion.FueRecibida)
        {
            return new ResultadoEmisionDTO
            {
                Exitoso = false,
                Mensajes = respuestaRecepcion.Mensajes.Select(m => m.Mensaje).ToList()
            };
        }

        // 6. Esperar y consultar autorización (el SRI puede tardar unos segundos)
        await Task.Delay(2000);
        var claveAcceso = facOrden.ClaveNumeroAutorizacion; // método que arma la clave de 49 dígitos
        var respuestaAutorizacion = await sriService.ConsultarAutorizacionAsync(claveAcceso);

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
}
