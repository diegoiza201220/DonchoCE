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
public class ProductoController(IUnitOfWork uow, ICacheService cache) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _ = cache.TryGet<IEnumerable<FacProductoDTO>>("PRODUCTOS_ALL", out var productos);
        //productos = productos.OrderBy(p => p.Grupo).ThenBy(p => p.OrdenAparicion);
        return Ok(new { productos });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var producto = await uow.FacProductoR.GetByIdAsync(id);
        return producto is null ? NotFound() : Ok(producto);
    }

    [HttpPost("actualizar")]
    public async Task<IActionResult> Update([FromBody] FacProductoDTO producto)
    {
        if (producto.Id <= 0) return BadRequest();

        FacProducto facproducto = producto.FromDTO();
        uow.FacProductoR.Update(facproducto);
        await uow.SaveChangesAsync();
        return Ok(CargarItemsEnCacheAsync());
    }

    [HttpPost("eliminar")]
    public async Task<IActionResult> Delete([FromBody] FacProductoDTO producto)
    {
        if (producto.Id <= 0) return BadRequest();
        FacProducto facproducto = producto.FromDTO();
        uow.FacProductoR.Delete(facproducto);
        try
        {
            await uow.SaveChangesAsync();
        }
        catch (Exception)
        {
            return BadRequest("Error: No se puede eliminar el producto");
            throw;
        }
        
        return Ok(CargarItemsEnCacheAsync());

    }

    [HttpPost("crear")]
    public async Task<IActionResult> Create([FromBody] FacProductoDTO producto)
    {
        if (producto.Id != 0) return BadRequest();

        FacProducto facproducto = producto.FromDTO();
        await uow.FacProductoR.AddAsync(facproducto);
        await uow.SaveChangesAsync();
        return Ok(CargarItemsEnCacheAsync());
    }

    private async Task<IEnumerable<FacProductoDTO>> CargarItemsEnCacheAsync()
    {
        var productos = uow.FacProductoR.GetAllDto();
        cache.SetPermanent("PRODUCTOS_ALL", productos);
        return productos;
    }

}
