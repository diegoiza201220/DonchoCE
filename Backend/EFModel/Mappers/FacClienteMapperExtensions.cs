using EFModel.DTO;
using EFModel.Models;

namespace EFModel.Mappers
{
    public static class FacClienteMapperExtensions
    {
        public static FacCliente FromDTO(this FacClienteDTO facClienteDTO)
        {
            return new FacCliente
            {
                Apellido = facClienteDTO.Apellido,
                CedulaRuc = facClienteDTO.CedulaRuc,
                Direccion = facClienteDTO.Direccion,
                Email = facClienteDTO.Email,
                FechaCumpleanios = facClienteDTO.FechaCumpleanios,
                Id = facClienteDTO.Id,
                Nombre = facClienteDTO.Nombre,
                TelefonoCelular = facClienteDTO.TelefonoCelular,
                UsuarioRegistro = facClienteDTO.UsuarioRegistro
            };
        }

        public static FacClienteDTO ToDTO(this FacCliente facCliente)
        {
            return new FacClienteDTO
            {
                Apellido = facCliente.Apellido,
                CedulaRuc = facCliente.CedulaRuc,
                Direccion = facCliente.Direccion,
                Email = facCliente.Email,
                FechaCumpleanios = facCliente.FechaCumpleanios,
                Id = facCliente.Id,
                Nombre = facCliente.Nombre,
                TelefonoCelular = facCliente.TelefonoCelular,
                UsuarioRegistro = facCliente.UsuarioRegistro
            };
        }
    }
}
