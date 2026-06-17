using ComprobantesElectronicos.Services;
using EFModel.DTO;
using EFModel.DTO.Reportes;
using EFModel.DTO.Request;
using EFModel.Interfaces;
using EFModel.Models;

namespace WebApiDonCho.Services
{
    public class ClienteService
    {
        private readonly IUnitOfWork _uow;
        private readonly ComprobanteService _comprobanteService;
        public ClienteService(IUnitOfWork uow, ComprobanteService comprobanteService)
        {
            _uow = uow;
            _comprobanteService = comprobanteService;
        }
        public async Task<FacCliente> AddCliente(FacClienteDTO cliente)
        {
            var faccliente = new FacCliente
            {
                Apellido = cliente.Apellido,
                CedulaRuc = cliente.CedulaRuc,
                Direccion = cliente.Direccion,
                Email = cliente.Email,
                FechaCumpleanios = cliente.FechaCumpleanios,
                FechaRegistro = DateOnly.FromDateTime(DateTime.Now),
                Nombre = cliente.Nombre,
                TelefonoCelular = cliente.TelefonoCelular,
                UsuarioRegistro = cliente.UsuarioRegistro
            };

            await _uow.FacClienteR.AddAsync(faccliente);
            await _uow.SaveChangesAsync();
            return faccliente;
        }

        private static void ActualizarSecuencia(FacSecuenciaDia secuencia, int fecha)
        {
            if (secuencia.Fecha == fecha)
                secuencia.Secuencia++;
            else
            {
                secuencia.Fecha = fecha;
                secuencia.Secuencia = 2;
            }
        }

        public async Task<IEnumerable<RptOrdenesPorFechasDTO>> GetOrdenesPorFechaAsync(RqOrdenesPorFechas rq)
        {
            var ordenes = await _uow.FacOrdenR.GetByFechas(rq.FechaIni, rq.FechaFin);

            return ordenes.Select(o => new RptOrdenesPorFechasDTO
            {
                ClienteId = o.Clienteid,
                Nombre = o.Cliente.Nombre,
                Apellido = o.Cliente.Apellido,
                FechaInteger = o.FechaInteger,
                Secuencial = o.Secuencial,
                Fecha = o.Fecha,
                TipoPago = o.TipoPago,
                TotalOrden = o.TotalOrden,
                UsuarioRegistro = o.UsuarioRegistro,
                EsFactura = o.EsFactura,
                NumeroFactura = o.NumeroFactura,
                DocumentoPago = o.DocumentoPago ?? ""
            });
        }

        public async Task<IEnumerable<RptProductosVendidosPorFechasDTO>> GetProductosVendidosPorFechaAsync(RqOrdenesPorFechas rq)
        {
            return await _uow.FacDetalleOrdenR.GetByFechasProductosVendidos(rq.FechaIni, rq.FechaFin);
        }
    }
}
