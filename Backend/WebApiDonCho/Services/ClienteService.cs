using ComprobantesElectronicos.Services;
using EFModel.DTO;
using EFModel.DTO.Reportes;
using EFModel.DTO.Request;
using EFModel.Interfaces;
using EFModel.Models;

namespace WebApiDonCho.Services
{
    public class ClienteService(IUnitOfWork uow, ComprobanteService comprobanteService)
    {
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

            await uow.FacClienteR.AddAsync(faccliente);
            await uow.SaveChangesAsync();
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
            var ordenes = await uow.FacOrdenR.GetByFechas(rq.FechaIni, rq.FechaFin, rq.SucursalId);

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
            return await uow.FacDetalleOrdenR.GetByFechasProductosVendidos(rq.FechaIni, rq.FechaFin, rq.SucursalId);
        }
    }
}
