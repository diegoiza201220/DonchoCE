using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EFModel.DTO;
//using AutoMapper;
using EFModel.DTO.Reportes;
using EFModel.DTO.Request;
using EFModel.Interfaces;
using EFModel.Models;
using WebApiDonCho.Services;

namespace WebApiDonCho.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdenController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly OrdenService _ordenService;

    public OrdenController(IUnitOfWork uow, OrdenService ordenService)
    {
        _uow = uow;
        _ordenService = ordenService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _uow.FacOrdenR.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var orden = await _uow.FacOrdenR.GetWithDetallesAsync(id);
        return orden is null ? NotFound() : Ok(orden);
    }

    [HttpGet("datospedido")]
    public async Task<IActionResult> GetDatosPedido()
    {
        var parametro = _uow.GenParametroR.GetById("CODIGO_TARIFA_IVA_FACTURAR");
        var tarifa = _uow.GenCatalogoDetalleR.GetById(Convert.ToInt16(parametro.Valor));
        DatosPedidoDTO datosPedidoDTO = new()
        {
            CodigoIva = Convert.ToInt16(parametro.Valor),
            ImpuestoPorcentaje = Convert.ToInt16(tarifa.Codigo.Replace("%",""))
        };

        return datosPedidoDTO is null ? NotFound() : Ok(datosPedidoDTO);
    }


    [HttpPost("facturar")]
    public async Task<IActionResult> Create([FromBody] FacOrdenDTO orden)
    {
        try
        {
            var facOrden = await _ordenService.FacturarAsync(orden);
            return CreatedAtAction(nameof(GetById), new { id = facOrden.Id }, facOrden);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("ordenesporfecha")]
    public async Task<IActionResult> OrdenesPorFecha([FromBody] RqOrdenesPorFechas detalle)
        => Ok(await _ordenService.GetOrdenesPorFechaAsync(detalle));

    [HttpPost("productosvendidosporfecha")]
    public async Task<IActionResult> ProductosVendidosPorFecha([FromBody] RqOrdenesPorFechas detalle)
        => Ok(await _ordenService.GetProductosVendidosPorFechaAsync(detalle));

    [HttpPost("facturasporfecha")]
    public async Task<IActionResult> FacturasPorFecha([FromBody] RqOrdenesPorFechas detalle)
    => Ok(await _ordenService.GetFacturasPorFechaAsync(detalle));
}
