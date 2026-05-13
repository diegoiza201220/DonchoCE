using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiDonCho.Interfaces;
using WebApiDonCho.Models;

namespace WebApiDonCho.Services
{
    public class AuthService(IConfiguration config)
    {
        private readonly IConfiguration _config = config;
        public string GenerarJwt(GenUsuario usuario)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            };

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credenciales,
                claims: claims
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
