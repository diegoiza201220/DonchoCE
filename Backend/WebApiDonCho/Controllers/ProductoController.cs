using EFModel.DTO;
using EFModel.Interfaces;
using EFModel.Mappers;
using EFModel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        _ = _cache.TryGet<IEnumerable<FacProductoDTO>>("PRODUCTOS_ALL", out var productos);
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

        FacProducto facproducto = producto.FromDTO();
        _uow.FacProductoR.Update(facproducto);
        await _uow.SaveChangesAsync();
        await CargarItemsEnCacheAsync();
        return NoContent();
    }

    [HttpDelete("eliminar")]
    public async Task<IActionResult> Delete([FromBody] FacProductoDTO producto)
    {
        if (producto.Id <= 0) return BadRequest();

        FacProducto facproducto = producto.FromDTO();
        _uow.FacProductoR.Delete(facproducto);
        await _uow.SaveChangesAsync();
        await CargarItemsEnCacheAsync();
        return NoContent();
    }

    [HttpPost("crear")]
    public async Task<IActionResult> Create([FromBody] FacProductoDTO producto)
    {
        if (producto.Id != 0) return BadRequest();

        FacProducto facproducto = producto.FromDTO();
        await _uow.FacProductoR.AddAsync(facproducto);
        await _uow.SaveChangesAsync();
        await CargarItemsEnCacheAsync();
        return CreatedAtAction(nameof(GetById), new { id = facproducto.Id }, facproducto);
    }

    private async Task CargarItemsEnCacheAsync()
    {
        var productos = await _uow.FacProductoR.GetAllDtoAsync();
        _cache.SetPermanent("PRODUCTOS_ALL", productos);
    }
}
