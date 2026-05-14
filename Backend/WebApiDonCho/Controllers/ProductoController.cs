using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EFModel.DTO;
using EFModel.Interfaces;
using EFModel.Models;
//using AutoMapper;

namespace WebApiDonCho.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductoController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly ICacheService _cache;

    public ProductoController(IUnitOfWork uow, ICacheService cache)
    {
        _uow = uow;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var productos = await _cache.GetOrCreatePermanentAsync("productos_all",() => _uow.FacProductoR.GetAllAsync());
        return Ok(new { productos });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var producto = await _uow.FacProductoR.GetByIdAsync(id);
        return producto is null ? NotFound() : Ok(producto);
    }

    [HttpPut("actualizar")]
    public async Task<IActionResult> Update([FromBody] FacProductoDTO producto)
    {
        if (producto.Id <= 0) return BadRequest();
        FacProducto facproducto = new()
        {
            Id = producto.Id,
            Activo = producto.Activo ?? false,
            CodigoIva = producto.CodigoIva ?? 0,
            Grupo = producto.Grupo,
            Nombre = producto.Nombre,
            OrdenAparicion = producto.OrdenAparicion,
            PedidoACocina = producto.PedidoACocina ?? false,
            Valor = producto.Valor
        };
        _uow.FacProductoR.Update(facproducto);
        await _uow.SaveChangesAsync();
        await CargarItemsEnCacheAsync();
        return NoContent();
    }

    [HttpDelete("eliminar")]
    public async Task<IActionResult> Delete([FromBody] FacProductoDTO producto)
    {
        FacProducto facproducto = new()
        {
            Id = producto.Id,
            Activo = producto.Activo ?? false,
            CodigoIva = producto.CodigoIva ?? 0,
            Grupo = producto.Grupo,
            Nombre = producto.Nombre,
            OrdenAparicion = producto.OrdenAparicion,
            PedidoACocina = producto.PedidoACocina ?? false,
            Valor = producto.Valor
        };
        if (producto.Id <= 0) return BadRequest();
        _uow.FacProductoR.Delete(facproducto);
        await _uow.SaveChangesAsync();
        await CargarItemsEnCacheAsync();
        return NoContent();
    }

    [HttpPost("crear")]
    public async Task<IActionResult> Create([FromBody] FacProductoDTO producto)
    {
        FacProducto facproducto = new()
        {
            Id = producto.Id,
            Activo = producto.Activo ?? false,
            CodigoIva = producto.CodigoIva ?? 0,
            Grupo = producto.Grupo,
            Nombre = producto.Nombre,
            OrdenAparicion = producto.OrdenAparicion,
            PedidoACocina = producto.PedidoACocina ?? false,
            Valor = producto.Valor
        };
        if (producto.Id != 0) return BadRequest();
        await _uow.FacProductoR.AddAsync(facproducto);
        await _uow.SaveChangesAsync();
        await CargarItemsEnCacheAsync();
        return CreatedAtAction(nameof(GetById), new { id = facproducto.Id }, facproducto);
    }

    private async Task CargarItemsEnCacheAsync()
    {
        var productos = await _uow.FacProductoR.GetAllAsync();
        _cache.SetPermanent("productos_all", productos);
    }
}
