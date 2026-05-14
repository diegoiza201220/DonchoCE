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

    [HttpGet("cliente/{clienteId:int}")]
    public async Task<IActionResult> GetByCliente(int clienteId)
        => Ok(await _uow.FacOrdenR.GetByClienteAsync(clienteId));

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
}
