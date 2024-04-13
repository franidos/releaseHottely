using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.DAL.Implementacion
{
    public class PedidoRepository : GenericRepository<Movimiento>, IPedidoRepository
    {
        private readonly DbventaContext _dbContext;

        public PedidoRepository(DbventaContext dbContext): base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Movimiento> Registrar(Movimiento entidad)//todo volar
        {
            Movimiento pedidoGenerado = new Movimiento();
            using(var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (DetalleMovimiento itemDV in entidad.DetalleMovimiento)
                    {
                        Producto productoEncontrado = _dbContext.Productos.Where(p => p.IdProducto == itemDV.IdProducto).First();
                        productoEncontrado.Stock = productoEncontrado.Stock + itemDV.Cantidad;
                        _dbContext.Productos.Update(productoEncontrado);
                    }
                    await _dbContext.SaveChangesAsync();

                    //NumeroCorrelativo correlativo = _dbContext.NumeroCorrelativos.Where(c => c.Gestion == "venta").First();
                    //correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    //correlativo.FechaActualizacion = DateTime.Now;

                    //_dbContext.NumeroCorrelativos.Update(correlativo);
                    //await _dbContext.SaveChangesAsync();

                    //string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                    //string numeroVenta = ceros + correlativo.UltimoNumero.ToString();
                    //numeroVenta = numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);

                    entidad.FechaRegistro = DateTime.Now;
                   // entidad.NumeroVenta = numeroVenta;
                    await _dbContext.Movimiento.AddAsync(entidad);
                    await _dbContext.SaveChangesAsync();

                    pedidoGenerado = entidad;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

            return pedidoGenerado;
        }

        //public async Task<List<DetalleMovimiento>> Reporte(DateTime fechaInicio, DateTime fechaFin)
        //{
        //    List<DetalleMovimiento> listaResumen = await _dbContext.DetalleMovimiento
        //        .Include(v => v.IdVentaNavigation)
        //        .ThenInclude(u => u.IdUsuarioNavigation)
        //        .Include(v => v.IdVentaNavigation)
        //        .ThenInclude(tdv => tdv.IdTipoDocumentoVentaNavigation)
        //        .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= fechaInicio.Date &&
        //        dv.IdVentaNavigation.FechaRegistro.Value.Date <= fechaFin.Date).ToListAsync();

        //    return listaResumen;
        //}
    }
}
