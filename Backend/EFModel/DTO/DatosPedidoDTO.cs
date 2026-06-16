using System.ComponentModel.DataAnnotations;

namespace EFModel.DTO
{
    public class DatosPedidoDTO
    {
        public int ImpuestoPorcentaje { get; set; }
        public int CodigoIva { get; set; }
        public int IdCatDetalleIva { get; set; }


    }
}