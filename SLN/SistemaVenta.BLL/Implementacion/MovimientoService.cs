using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.BLL.Implementacion
{
    public class MovimientoService : IMovimientoService
    {
        private readonly IGenericRepository<Producto> _repositorioProducto;
        private readonly IMovimientoRepository _repositorioMovimiento;

        public MovimientoService(IGenericRepository<Producto> repositorioProducto, IMovimientoRepository repositorioMovimiento)
        {
            _repositorioProducto = repositorioProducto;
            _repositorioMovimiento = repositorioMovimiento;
        }

        public async Task<List<Producto>> ObtenerProductos(int idEstablishment, string busqueda)
        {
            IQueryable<Producto> query = await _repositorioProducto.Consultar(
                p => p.EsActivo == true && 
                p.Stock > 0 && p.IdEstablishment == idEstablishment &&
                string.Concat(p.CodigoBarra, p.Marca, p.Descripcion).Contains(busqueda)
                );

            return query.Include(c => c.IdCategoriaProductoNavigation).ToList();
        }

        public async Task<Movimiento> Registrar(Movimiento entidad)
        {

            try
            {
                return await _repositorioMovimiento.Registrar(entidad);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<Movimiento>> Historial(int idEstablishment,string numeroMovimiento, string buscarPorTipo, string fechaInicio, string fechaFin)
        {
            IQueryable<Movimiento> query = await _repositorioMovimiento.Consultar(x=>x.IdUsuarioNavigation.IdEstablishment == idEstablishment);
            fechaInicio = fechaInicio is null? "" : fechaInicio;
            fechaFin = fechaFin is null ? "" : fechaFin;
            buscarPorTipo = buscarPorTipo is null ? "E" : buscarPorTipo == "salida" ? "S" : "E";


            if (fechaInicio != "" && fechaFin != "")
            {
                DateTime fecha_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-CO"));
                DateTime fecha_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-CO"));

                return query.Where(v => 
                        v.FechaRegistro.Value.Date >= fecha_inicio.Date &&
                        v.FechaRegistro.Value.Date <= fecha_fin.Date &&
                        (v.IdTipoDocumentoMovimientoNavigation.Naturaleza == buscarPorTipo)

                    )
                    .Include(tDoc => tDoc.IdTipoDocumentoMovimientoNavigation)
                    .Include(usu => usu.IdUsuarioNavigation)
                    .Include(det => det.DetalleMovimiento)
                    .ToList();
            }
            else
            {
                return query.Where(v => v.NumeroMovimiento == numeroMovimiento)
                   .Include(tDoc => tDoc.IdTipoDocumentoMovimientoNavigation)
                   .Include(usu => usu.IdUsuarioNavigation)
                   .Include(det => det.DetalleMovimiento)
                   .ToList();
            }

        }

        public async Task<Movimiento> Detalle(string numeroMovimiento, int idEstablishment)
        {
            IQueryable<Movimiento> query = await _repositorioMovimiento.Consultar(v => v.IdEstablishment == idEstablishment && v.NumeroMovimiento == numeroMovimiento);
            return query
                   .Include(tDoc => tDoc.IdTipoDocumentoMovimientoNavigation)
                   .Include(usu => usu.IdUsuarioNavigation)
                   .Include(det => det.DetalleMovimiento)
                   .First();
        }

        public async Task<List<DetalleMovimiento>> Reporte(int idEstablishment, string fechaInicio, string fechaFin)
        {
            DateTime fecha_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-CO"));
            DateTime fecha_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-CO"));

            List<DetalleMovimiento> lista = await _repositorioMovimiento.Reporte(idEstablishment,fecha_inicio, fecha_fin);
            return lista;

        }

        public async Task<List<Movimiento>> GetMovimientosBooking(int idMvtoRel)
        {
            IQueryable<Movimiento> query = await _repositorioMovimiento.Consultar(x=>x.IdMovimientoRel == idMvtoRel);
            return query.Include(tDoc => tDoc.DetalleCaja).Include(x=>x.IdTipoDocumentoMovimientoNavigation).ToList();
        }

        public async Task<Movimiento> Editar(Movimiento entidad)
        {
            try
            {
                Movimiento registro = await _repositorioMovimiento.Obtener(c => c.IdMovimiento == entidad.IdMovimiento);
                registro.TotalRoom = entidad.TotalRoom;

                bool respuesta = await _repositorioMovimiento.Editar(registro);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo editar el Movimiento");
                }

                return registro;
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
