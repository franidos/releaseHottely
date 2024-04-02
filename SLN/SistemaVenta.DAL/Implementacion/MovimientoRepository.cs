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
    public class MovimientoRepository : GenericRepository<Movimiento>, IMovimientoRepository
    {
        private readonly DbventaContext _dbContext;

        public MovimientoRepository(DbventaContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Movimiento> Registrar(Movimiento entidad)
        {
            Movimiento ventaGenerada = new Movimiento();
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var tipoDoc = string.Empty;

                    if (entidad.DetalleMovimiento.Count > 0)
                    {

                        foreach (DetalleMovimiento itemDV in entidad.DetalleMovimiento)
                        {
                            Producto productoEncontrado = new Producto();
                            productoEncontrado = _dbContext.Productos.Where(p => p.IdProducto == itemDV.IdProducto).First();
                            //tipoDoc = string.Empty;

                            if (entidad.IdTipoDocumentoMovimiento == 3 || entidad.IdTipoDocumentoMovimiento == 4)
                            {
                                productoEncontrado.Stock = productoEncontrado.Stock + itemDV.Cantidad;
                                productoEncontrado.PrecioCompra = itemDV.Precio;
                                tipoDoc = "pedido";
                            }

                            else
                            {
                                productoEncontrado.Stock = productoEncontrado.Stock - itemDV.Cantidad;
                                tipoDoc = "venta";
                            }

                            _dbContext.Productos.Update(productoEncontrado);
                        }

                        await _dbContext.SaveChangesAsync();

                    }
                    else
                    {
                        tipoDoc = "reserva";

                    }

                    NumeroCorrelativo correlativo = _dbContext.NumeroCorrelativos.Where(c => c.IdEstablishment == entidad.IdEstablishment && c.Gestion == tipoDoc).First();

                    correlativo.UltimoNumero = correlativo.UltimoNumero == null ? 1 : correlativo.UltimoNumero + 1;
                    correlativo.FechaActualizacion = DateTime.Now;

                    _dbContext.NumeroCorrelativos.Update(correlativo);
                    await _dbContext.SaveChangesAsync();

                    string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                    string numeroMovimiento = ceros + correlativo.UltimoNumero.ToString();
                    numeroMovimiento = numeroMovimiento.Substring(numeroMovimiento.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);
                    entidad.FechaRegistro = DateTime.Now;
                    entidad.NumeroMovimiento = numeroMovimiento;
                    await _dbContext.Movimiento.AddAsync(entidad);
                    await _dbContext.SaveChangesAsync();

                    ventaGenerada = entidad;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

            return ventaGenerada;
        }

        public async Task<List<DetalleMovimiento>> Reporte(int idEstablishment, DateTime fechaInicio, DateTime fechaFin)
        {
            List<DetalleMovimiento> listaResumen = await _dbContext.DetalleMovimiento
                .Include(v => v.IdMovimientoNavigation)
                .ThenInclude(u => u.IdUsuarioNavigation)
                .Include(v => v.IdMovimientoNavigation)
                .ThenInclude(tdv => tdv.IdTipoDocumentoMovimientoNavigation)
                .Where(dv => dv.IdMovimientoNavigation.FechaRegistro.Value.Date >= fechaInicio.Date &&
                dv.IdMovimientoNavigation.FechaRegistro.Value.Date <= fechaFin.Date && dv.IdMovimientoNavigation.IdUsuarioNavigation.IdEstablishment == idEstablishment).ToListAsync();


            return listaResumen;
        }
    }
}
