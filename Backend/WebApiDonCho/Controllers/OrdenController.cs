using EFModel.DTO;
using EFModel.DTO.Request;
using EFModel.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiDonCho.Services;
namespace WebApiDonCho.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdenController(IUnitOfWork uow, OrdenService ordenService, ICacheService cache) : ControllerBase
{
    private readonly IUnitOfWork _uow = uow;
    private readonly OrdenService _ordenService = ordenService;
    private readonly ICacheService _cache = cache;

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
        _cache.TryGet("PORCENTAJE_IVA", out string porcentajeIva);
        _cache.TryGet("CODIGO_IVA", out string codigoIva);
        _cache.TryGet("ID_CATDETALLE_IVA", out int idCatDetalleIva);

        DatosPedidoDTO datosPedidoDTO = new()
        {
            CodigoIva = Convert.ToInt16(codigoIva),
            ImpuestoPorcentaje = Convert.ToInt16(porcentajeIva),
            IdCatDetalleIva = idCatDetalleIva
        };

        return datosPedidoDTO is null ? NotFound() : Ok(datosPedidoDTO);
    }


    [HttpPost("facturar")]
    public async Task<IActionResult> CreateFactura([FromBody] FacOrdenDTO orden)
    {
        try
        {
            var facOrden = await _ordenService.GenerarFacturaAsync(orden);
            return CreatedAtAction(nameof(GetById), new { id = facOrden.Id }, facOrden);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost("devolver")]
    public async Task<IActionResult> CreateNotaCredito([FromBody] FacOrdenDTO orden)
    {
        try
        {
            var facOrden = await _ordenService.GenerarNotaCreditoAsync(orden);
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

    [HttpPost("documentosporfecha")]
    public async Task<IActionResult> DocumentosPorFecha([FromBody] RqOrdenesPorFechas detalle)
    => Ok(await _ordenService.GetDocumentosPorFechaAsync(detalle));
}
