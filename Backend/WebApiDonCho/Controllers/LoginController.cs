using EFModel.Interfaces;
using EFModel.Models;
using Microsoft.AspNetCore.Mvc;
using WebApiDonCho.Services;

namespace WebApiDonCho.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly AuthService _auth;

    public LoginController(IUnitOfWork uow, AuthService auth)
    {
        _uow = uow;
        _auth = auth;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _uow.GenUsuarioR.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var usuario = await _uow.GenUsuarioR.GetByIdAsync(id);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpPost("validatelogin")]
    public async Task<IActionResult> ValidateLogin([FromBody] GenUsuario usuario)
    {
        if (usuario is null) return BadRequest();
        var uv = await _uow.GenUsuarioR.ValidateLogin(usuario.Nombre, usuario.Password);
        if (uv is null) return Unauthorized();
        var token = _auth.GenerarJwt(uv);
        return Ok(new { token, user = uv.Nombre });
    }

}
