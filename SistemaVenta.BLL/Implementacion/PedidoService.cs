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
    public class PedidoService : IPedidoService
    {
        private readonly IGenericRepository<Proveedor> _repositorioProveedor;
        private readonly IGenericRepository<Producto> _repositorioProducto;
       // private readonly IGenericRepository<Movimiento> _repositorioPedido;
        private readonly IMovimientoRepository _repositorioMovimiento;
        public PedidoService(IGenericRepository<Proveedor> repositorioProveedor, IGenericRepository<Producto> repositorioProducto,
            IMovimientoRepository repositorioMovimiento)
        {
            _repositorioProveedor = repositorioProveedor;
            _repositorioProducto = repositorioProducto;
            _repositorioMovimiento = repositorioMovimiento;
        }

        public async Task<List<Proveedor>> ObtenerProveedor(int idEstablishment,string busqueda)
        {
            IQueryable<Proveedor> query = await _repositorioProveedor.Consultar(
                p => p.EsActivo == true && p.IdEstablishment == idEstablishment &&
               // p.Stock > 0 && 
                string.Concat(p.NIT, p.Nombre, p.Contacto).Contains(busqueda)
                );

            return query.ToList();
        }

        public async Task<List<Producto>> ObtenerProductos(int idEstablishment,string busqueda)
        {
            IQueryable<Producto> query = await _repositorioProducto.Consultar(
                p => p.EsActivo == true && p.IdEstablishment == idEstablishment &&
                p.IdProveedor == Int32.Parse(busqueda)
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
        //public async Task<List<Pedido>> Historial(string numeroPedidoProveedorProveedor, string fechaInicio, string fechaFin)
        //{
        //    IQueryable<Pedido> query = await _repositorioPedidoProveedorProveedor.Consultar();
        //    fechaInicio = fechaInicio is null? "" : fechaInicio;
        //    fechaFin = fechaFin is null ? "" : fechaFin;

        //    if (fechaInicio != "" && fechaFin != "")
        //    {
        //        DateTime fecha_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-CO"));
        //        DateTime fecha_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-CO"));

        //        return query.Where(v => 
        //                v.FechaRegistro.Value.Date >= fecha_inicio.Date &&
        //                v.FechaRegistro.Value.Date <= fecha_fin.Date
        //            )
        //            .Include(tDoc => tDoc.IdTipoDocumentoPedidoProveedorProveedorNavigation)
        //            .Include(usu => usu.IdUsuarioNavigation)
        //            .Include(det => det.DetallePedidoProveedorProveedor)
        //            .ToList();
        //    }
        //    else
        //    {
        //        return query.Where(v => v.NumeroPedidoProveedorProveedor == numeroPedidoProveedorProveedor)
        //           .Include(tDoc => tDoc.IdTipoDocumentoVentaNavigation)
        //           .Include(usu => usu.IdUsuarioNavigation)
        //           .Include(det => det.DetalleMovimiento)
        //           .ToList();
        //    }

        //}

        //public async Task<Movimiento> Detalle(string numeroMovimiento)
        //{
        //    IQueryable<Movimiento> query = await _repositorioMovimiento.Consultar(v => v.NumeroMovimiento == numeroMovimiento);
        //    return query
        //           .Include(tDoc => tDoc.IdTipoDocumentoMovimientoNavigation)
        //           .Include(usu => usu.IdUsuarioNavigation)
        //           .Include(det => det.DetalleMovimiento)
        //           .First();
        //}

        //public async Task<List<DetalleMovimiento>> Reporte(string fechaInicio, string fechaFin)
        //{
        //    DateTime fecha_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-CO"));
        //    DateTime fecha_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-CO"));

        //    List<DetalleMovimiento> lista = await _repositorioMovimiento.Reporte(fecha_inicio, fecha_fin);
        //    return lista;

        //}
    }
}
