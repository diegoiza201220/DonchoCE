using EFModel.DTO;
using EFModel.Models;

namespace EFModel.Mappers
{
    public static class FacProductoMapperExtensions
    {
        public static FacProducto FromDTO(this FacProductoDTO producto)
        {
            return new FacProducto
            {
                Id = producto.Id,
                Activo = producto.Activo,
                CodigoIva = producto.CodigoIva,
                Grupo = producto.Grupo,
                Nombre = producto.Nombre,
                OrdenAparicion = producto.OrdenAparicion,
                PedidoACocina = producto.PedidoACocina ?? false,
                Valor = producto.Valor,
                ValorDoncho = producto.ValorDoncho
            };
        }
    }
}
