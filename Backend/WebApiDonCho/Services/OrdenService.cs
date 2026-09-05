using ComprobantesElectronicos.Services;
using EFModel.DTO;
using EFModel.DTO.Reportes;
using EFModel.DTO.Request;
using EFModel.Interfaces;
using EFModel.Mappers;
using EFModel.Models;
using Utils;
using WebApiDonCho.Helpers.ComprobantesElectronicos;

namespace WebApiDonCho.Services
{
    public class OrdenService(IUnitOfWork uow, ComprobanteService comprobanteService, ICacheService cache)
    {
        public async Task<FacOrdenDTO> GenerarFacturaAsync(FacOrdenDTO facOrdenDTO)
        {


            var facOrden = facOrdenDTO.FromDTO();
            var secuencia = await uow.FacSecuenciaDiaR.GetBySucursalIdAsync(facOrden.Sucursalid) ?? throw new InvalidOperationException("No existe registro de secuencia del día.");

            ActualizarSecuenciaOrdenesDiaria(secuencia, facOrdenDTO.FechaInteger);

            uow.FacSecuenciaDiaR.Update(secuencia);
            await uow.FacOrdenR.AddAsync(facOrden);
            await uow.SaveChangesAsync();

            facOrdenDTO = facOrden.ToDTO();
            CompletarInformacionDetalleOrden(facOrdenDTO);
            CompletarInformaciónCliente(facOrdenDTO);
            CompletarInformacionTributaria(facOrdenDTO);

            if (!facOrdenDTO.EsFactura) //retornamos desde aquí si no es factura, para evitar el proceso de emisión electrónica y envío de correo
            {
                return facOrdenDTO;
            }

            facOrdenDTO.CodDoc = "01";
            CelSecuenciaSri celSecuenciaSri = uow.CelSecuenciasSriR.GetByTipoDocumento("01", facOrden.Sucursalid) ?? throw new InvalidOperationException($"Secuencia SRI no encontrada para la sucursal con id: {facOrden.Sucursalid}");
            _ = cache.TryGet(Constantes.CELINFOTRIBUTARIA, out CelInfoTributaria celInfoTributaria);
            InfoTributariaHelper.SetInformacion(facOrdenDTO, facOrden, celInfoTributaria, celSecuenciaSri, esProduccion: false, (int)ComprobantesElectronicos.Enums.CodigoDocumento.Factura);
            CelLogDocumento celLogDocumento = CelLogDocumentoHelper.CrearLogInicial(facOrdenDTO);
            await uow.CelLogDocumentoR.AddAsync(celLogDocumento);
            celSecuenciaSri.SecuenciaActual++;
            uow.CelSecuenciasSriR.Update(celSecuenciaSri);
            await uow.SaveChangesAsync();

            _ = await comprobanteService.EmitirFacturaAsync(facOrdenDTO, celLogDocumento);
            //CompletarInformacionDetalleOrden(facOrdenDTO);
            //CompletarInformaciónCliente(facOrdenDTO);
            //CompletarInformacionTributaria(facOrdenDTO);
            return facOrdenDTO;
        }

        private void CompletarInformacionDetalleOrden(FacOrdenDTO facOrdenDTO)
        {
            var productos = uow.FacDetalleOrdenR.GetByOrdenAsync(facOrdenDTO.Id).Result;
            foreach (var detalle in facOrdenDTO.FacDetalleOrdens)
            {
                var producto = productos.FirstOrDefault(p => p.Productoid == detalle.ProductoId);
                if (producto != null)
                {
                    detalle.Nombre = producto.Producto.Nombre;
                }
            }
        }

        private void CompletarInformaciónCliente(FacOrdenDTO facOrdenDTO)
        {
            var cliente = uow.FacClienteR.GetByIdAsync(facOrdenDTO.Clienteid).Result;
            if (cliente != null)
            {
                facOrdenDTO.Cliente = cliente.ToDTO();
                facOrdenDTO.clienteNombre = $"{cliente.Nombre} {cliente.Apellido}";
                facOrdenDTO.clienteRuc = cliente.CedulaRuc;
            }
        }

        private void CompletarInformacionTributaria(FacOrdenDTO facOrdenDTO)
        {
            _ = cache.TryGet(Constantes.CELINFOTRIBUTARIA, out CelInfoTributaria celInfoTributariaNotaVenta);
            facOrdenDTO.NombreComercial = celInfoTributariaNotaVenta.NombreComercial;
            facOrdenDTO.SucursalNombre = uow.GenSucursalR.GetById(facOrdenDTO.Sucursalid).Nombre;
        }
        public async Task<FacOrden> GenerarNotaCreditoAsync(FacOrdenDTO orden)
        {
            if (!orden.EsFactura) //retornamos desde aquí si no es factura, para evitar el proceso de emisión electrónica y envío de correo
            {
                return null;
            }
            //var secuencia = await uow.FacSecuenciaDiaR.GetSecuenciaAsync() ?? throw new InvalidOperationException("No existe registro de secuencia del día.");

            //var codigoPorcentaje = Convert.ToInt16(uow.GenCatalogoDetalleR.GetById(orden.ImpuestoCodigoPorcentaje).Valor);
            FacOrden facOrden = await uow.FacOrdenR.GetByIdAsync(orden.Id);
            //orden.ImpuestoCodigoPorcentaje = codigoPorcentaje;
            //orden.FacDetalleOrdens.ForEach(d => d.ImpuestoCodigoPorcentaje = codigoPorcentaje);
            //var facOrden = orden.ToDTO();

            //ActualizarSecuenciaOrdenesDiaria(secuencia, orden.FechaInteger);

            //uow.FacSecuenciaDiaR.Update(secuencia);


            //await uow.FacOrdenR.AddAsync(facOrden);
            //await uow.SaveChangesAsync();

            //if (!orden.EsFactura) //retornamos desde aquí si no es factura, para evitar el proceso de emisión electrónica y envío de correo
            //{
            //    return facOrden;
            //}

            //orden.CodDoc = "01";
            CelSecuenciaSri celSecuenciaSri = uow.CelSecuenciasSriR.GetByTipoDocumento("03",orden.Sucursalid) ?? throw new InvalidOperationException($"Secuencia SRI no encontrada para la sucursal con id: {facOrden.Sucursalid}");
            CelInfoTributaria celInfoTributaria = cache.GetOrCreatePermanent("CELINFOTRIBUTARIA", () => uow.CelInfoTributariaR.GetById(1));
            InfoTributariaHelper.SetInformacion(orden, facOrden, celInfoTributaria, celSecuenciaSri, esProduccion: false, (int)ComprobantesElectronicos.Enums.CodigoDocumento.NotaCredito);
            CelLogDocumento celLogDocumento = CelLogDocumentoHelper.CrearLogInicial(orden);
            await uow.CelLogDocumentoR.AddAsync(celLogDocumento);
            celSecuenciaSri.SecuenciaActual++;
            uow.CelSecuenciasSriR.Update(celSecuenciaSri);
            await uow.SaveChangesAsync();
            //_ = await comprobanteService.emi(orden, celLogDocumento);
            return facOrden;
        }
        private static void ActualizarSecuenciaOrdenesDiaria(FacSecuenciaDia secuencia, int fecha)
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

        public async Task<IEnumerable<RptFacturasPorFechasDTO>> GetFacturasPorFechaAsync(RqOrdenesPorFechas rq)
        {
            return await uow.FacOrdenR.GetFacturasPorFecha(rq.FechaIni, rq.FechaFin, rq.SucursalId);
        }

        public async Task<IEnumerable<RptDocumentosPorFechasDTO>> GetDocumentosPorFechaAsync(RqOrdenesPorFechas rq)
        {
            return await uow.FacOrdenR.GetDocumentosPorFecha(rq.FechaIni, rq.FechaFin, rq.SucursalId);
        }
    }
}
